using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CSharpFunctionalExtensions;
using gec.Infrastructure.Common;
using Microsoft.IdentityModel.Tokens;

namespace gec.Infrastructure.Lti;

public class JwtValidationService : IJwtValidationService
{
    private readonly AppSettingsService _appSettings;

    public JwtValidationService(AppSettingsService appSettings)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    }

    public async Task<Result<ClaimsPrincipal>> ValidateTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return Result.Failure<ClaimsPrincipal>("El token proporcionado es nulo o vacío.");
        }

        try
        {
            var validationParameters = await BuildTokenValidationParametersAsync();
            var handler = new JwtSecurityTokenHandler();

            // Validar el token
            var principal = handler.ValidateToken(token, validationParameters, out _);
            return Result.Success(principal);
        }
        catch (SecurityTokenExpiredException ex)
        {
            return Result.Failure<ClaimsPrincipal>("El token ha expirado.");
        }
        catch (SecurityTokenException ex)
        {
            return Result.Failure<ClaimsPrincipal>($"El token no es válido: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.Failure<ClaimsPrincipal>("Ocurrió un error al validar el token.");
        }
    }

    private async Task<TokenValidationParameters> BuildTokenValidationParametersAsync()
    {
        // var signingKeys = await GetSigningKeysFromJwksAsync($"{_appSettings.LtiUrlBase}/.well-known/jwks.json");
        var signingKeys = await GetSigningKeysFromJwksAsync($"{_appSettings.LtiUrlBase}/api/lti/security/jwks");

        return new TokenValidationParameters
        {
            ValidIssuer = _appSettings.LtiValidIssuer, // URL base del emisor
            ValidAudience = _appSettings.LtiClientId, // ID de cliente configurado
            IssuerSigningKeys = signingKeys,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(5) // Tolerancia en la validación de tiempos
        };
    }

    private async Task<IEnumerable<SecurityKey>> GetSigningKeysFromJwksAsync(string jwksUrl)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetStringAsync(jwksUrl);

            var jwks = new JsonWebKeySet(response);
            return jwks.Keys;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("No se pudieron obtener las claves de firma.", ex);
        }
    }
}