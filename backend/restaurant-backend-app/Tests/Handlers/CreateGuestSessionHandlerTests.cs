using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using APIHandler.Models.GuestSessions;
using APIHandler.Services.GuestSessions;
using Moq;
using System.Text.Json;

namespace APIHandler.Tests.Handlers;

public class CreateGuestSessionHandlerTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task HandleRequest_WithoutOptionalBearer_CreatesGuestSession(bool includeNonBearerAuthorization)
    {
        var responseBody = new CreatGuestSessionResponse("session-id", "session-token");
        var guestSessionService = new Mock<IGuestSessionService>();
        guestSessionService
            .Setup(service => service.CreateGuestSessionAsync(
                It.Is<CreateGuestSessionRequest>(request =>
                    request.GroupReservationId == "group-id" &&
                    request.GuestName == "Owner Name"),
                null))
            .ReturnsAsync(responseBody);

        var handler = new APIHandler.Handlers.GuestSessions.CreateGuestSessionHandler(guestSessionService.Object);
        var request = new APIGatewayProxyRequest
        {
            Body = """{"groupReservationId":"group-id","guestName":"Owner Name"}""",
            Headers = includeNonBearerAuthorization
                ? new Dictionary<string, string> { ["Authorization"] = "Basic ignored" }
                : new Dictionary<string, string>()
        };
        var lambdaContext = new Mock<ILambdaContext>();
        lambdaContext.SetupGet(context => context.Logger).Returns(Mock.Of<ILambdaLogger>());

        var response = await handler.HandleRequest(request, lambdaContext.Object);

        Assert.Equal(200, response.StatusCode);
        using var document = JsonDocument.Parse(response.Body);
        Assert.Equal("session-id", document.RootElement.GetProperty("SessionId").GetString());
        Assert.Equal("session-token", document.RootElement.GetProperty("SessionToken").GetString());
        guestSessionService.Verify(
            service => service.CreateGuestSessionAsync(It.IsAny<CreateGuestSessionRequest>(), null),
            Times.Once);
    }
}
