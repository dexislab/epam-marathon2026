using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using APIHandler.Models.GuestSessions;
using APIHandler.Services.GuestSessions;
using Moq;
using System.Text.Json;

namespace APIHandler.Tests.Handlers;

public class DeleteGuestSessionHandlerTests
{
    [Fact]
    public async Task HandleRequest_ReturnsCamelCaseUpdatedSnapshot()
    {
        var snapshot = new GetAllSessionsResponse([], 0m);
        var guestSessionService = new Mock<IGuestSessionService>();
        guestSessionService
            .Setup(service => service.DeleteGuestSessionAsync("owner-id", "group-id", "session-id"))
            .ReturnsAsync(snapshot);
        var handler = new APIHandler.Handlers.GuestSessions.DeleteGuestSessionHandler(guestSessionService.Object);
        var request = new APIGatewayProxyRequest
        {
            PathParameters = new Dictionary<string, string>
            {
                ["groupReservationId"] = "group-id",
                ["guestSessionId"] = "session-id"
            },
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
            {
                Authorizer = new APIGatewayCustomAuthorizerContext
                {
                    Claims = new Dictionary<string, string> { ["sub"] = "owner-id" }
                }
            }
        };
        var lambdaContext = new Mock<ILambdaContext>();
        lambdaContext.SetupGet(context => context.Logger).Returns(Mock.Of<ILambdaLogger>());

        var response = await handler.HandleRequest(request, lambdaContext.Object);

        Assert.Equal(200, response.StatusCode);
        using var document = JsonDocument.Parse(response.Body);
        Assert.True(document.RootElement.TryGetProperty("participants", out _));
        Assert.True(document.RootElement.TryGetProperty("total", out _));
        Assert.False(document.RootElement.TryGetProperty("Participants", out _));
    }
}