using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using APIHandler.Exceptions;
using APIHandler.Helpers;
using APIHandler.Models.GuestSessions;
using APIHandler.Services.GuestSessions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace APIHandler.Handlers.GuestSessions;

public class CreateGuestSessionHandler(IGuestSessionService guestSessionService) : IApiRequestHandler
{
    public async Task<APIGatewayProxyResponse> HandleRequest(APIGatewayProxyRequest request, ILambdaContext context)
    {
        if (!RequestBodyHelper.TryParseAndValidate(request, context, out CreateGuestSessionRequest? requestBody, out APIGatewayProxyResponse? errorResponse))
            return errorResponse;

        string? authenticatedUserId = null;
        string? authorization = request.Headers?.FirstOrDefault(header =>
            string.Equals(header.Key, "Authorization", StringComparison.OrdinalIgnoreCase)).Value;
        const string bearerPrefix = "Bearer ";
        if (authorization?.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase) == true)
        {
            string token = authorization[bearerPrefix.Length..].Trim();
            if (token.Length > 0)
            {
                try
                {
                    var identity = await CognitoIdTokenValidator.ValidateAsync(token);
                    authenticatedUserId = identity?.FindFirst("sub")?.Value;
                }
                catch (CognitoMetadataUnavailableException exception)
                {
                    return ResponseBuilder.CreateErrorResponse(
                        503,
                        "Identity provider is temporarily unavailable.",
                        context.Logger,
                        exception);
                }
                catch (Microsoft.IdentityModel.Tokens.SecurityTokenException)
                {
                    authenticatedUserId = null;
                }
            }
        }

        try
        {
            var result = await guestSessionService.CreateGuestSessionAsync(requestBody, authenticatedUserId);
            return ResponseBuilder.CreateSuccessResponse(result);
        }
        catch (ValidationException exception)
        {
            return ResponseBuilder.CreateErrorCodeResponse(exception);
        }
    }
}
