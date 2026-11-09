using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using APIHandler.Constants;
using APIHandler.Exceptions;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace APIHandler.Helpers;

public static class CognitoIdTokenValidator
{
    private static readonly string Issuer =
        $"https://cognito-idp.{ResourceNames.Region}.amazonaws.com/{ResourceNames.UserPoolId}";

    private static readonly ConfigurationManager<OpenIdConnectConfiguration> ConfigManager =
        new($"{Issuer}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());

    public static async Task<ClaimsIdentity?> ValidateAsync(string token)
    {
        OpenIdConnectConfiguration config;
        try
        {
            config = await ConfigManager.GetConfigurationAsync();
        }
        catch (Exception exception)
        {
            throw new CognitoMetadataUnavailableException(exception);
        }

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = ResourceNames.ClientId,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = config.SigningKeys,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        var result = await handler.ValidateTokenAsync(token, parameters);

        if (!result.IsValid || result.ClaimsIdentity?.FindFirst("token_use")?.Value != "id")
            return null;

        return result.ClaimsIdentity;
    }
}