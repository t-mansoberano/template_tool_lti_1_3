using System.Security.Claims;
using System.Text.Json;
using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Lti;
using gec.Application.Contracts.Infrastructure.Lti.Models;
using gec.Infrastructure.Common;

namespace gec.Infrastructure.Lti;

public class LtiService : ILtiService
{
    private readonly AppSettingsService _appSettings;
    private readonly IJwtValidationService _jwtValidationService;

    public LtiService(AppSettingsService appSettings, IJwtValidationService jwtValidationService)
    {
        _appSettings = appSettings;
        _jwtValidationService = jwtValidationService;
    }

    public Result<string> BuildAuthorizationUrl(LoginInitiationResponse form)
    {
        var validate = form.Validate();
        if (validate.IsFailure)
            return validate;

        var state = Guid.NewGuid().ToString();
        var nonce = Guid.NewGuid().ToString();

        var queryParams = BuildQueryParameters(form, state, nonce);

        var redirectUrl = $"{_appSettings.LtiUrlBase}/api/lti/authorize_redirect?" + string.Join("&", queryParams);
        return Result.Success(redirectUrl);
    }

    private List<string> BuildQueryParameters(LoginInitiationResponse form, string state, string nonce)
    {
        return new List<string>
        {
            "scope=openid",
            "response_type=id_token",
            $"client_id={form.ClientId}",
            $"redirect_uri={Uri.EscapeDataString(_appSettings.LtiRedirectUri)}",
            $"login_hint={Uri.EscapeDataString(form.LoginHint)}",
            $"lti_message_hint={Uri.EscapeDataString(form.LtiMessageHint)}",
            $"state={Uri.EscapeDataString(state)}",
            "response_mode=form_post",
            $"nonce={Uri.EscapeDataString(nonce)}",
            "prompt=none"
        };
    }

    public async Task<Result<LtiContext>> HandleRedirectAsync(Dictionary<string, string> form)
    {
        var authenticationResponse = new AuthenticationResponse(form);
        var validate = authenticationResponse.Validate();
        if (validate.IsFailure)
            return Result.Failure<LtiContext>(validate.Error);

        var tokenResult = await _jwtValidationService.ValidateTokenAsync(authenticationResponse.IdToken);
        if (tokenResult.IsFailure)
            return Result.Failure<LtiContext>(tokenResult.Error);

        var claims = ParseClaims(tokenResult.Value.Claims);

        var customData = ExtractCustomData(claims);
        var user = BuildUser(claims, customData);
        var course = BuildCourse(claims, customData);

        return Result.Success(new LtiContext { User = user, Course = course });
    }

    private Dictionary<string, List<string>> ParseClaims(IEnumerable<Claim> claims)
    {
        return claims
            .GroupBy(c => c.Type)
            .ToDictionary(g => g.Key, g => g.Select(v => v.Value).ToList());
    }

    private Dictionary<string, JsonElement> ExtractCustomData(Dictionary<string, List<string>> claims)
    {
        var customClaimJson = claims.GetValueOrDefault(ClaimTypes.Custom)?.FirstOrDefault();
        return !string.IsNullOrEmpty(customClaimJson)
            ? JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(customClaimJson) ?? new()
            : new();
    }

    public Result<string> GetJwks()
    {
        // Define el JSON que quieres devolver
        var jwks = new
        {
            keys = new[]
            {
                new
                {
                    kty = "RSA", // Tipo de clave
                    use = "sig", // Uso de la clave (firma)
                    alg = "RS256", // Algoritmo de firma
                    n =
                        "wwo9Um22g6yojCGpqgk-I7KlEyUExHP8iHr6rrBqgEX8QD99nEtjtgEDb2Dc3xWkcMj1E57K7SAym5-7HXBu7b6dURwjv3KJk_FxuSDK43MNKiJsn1IOuToXJwcE-O6Jz67zOAL4Vnz-s1mIsLSWUkYeVg4l9ixfj3Ddo37TWn75WQbN2TiFGQzPaJXDeBHDOhvwClCCAXnBcS0PlLrujyqAvyNOdqv-7oMlaTdLxyI1RNLLHMDNxjohadJlEd77n-p6id34RLGaFeTW5hj8DhcCNrE9FmysOCdZy9Fj2DLVu0FvKfqd-X3NN-jpZZO1uEZd-q8GzqXqHQcACNFiRw", // Modulus
                    e = "AQAB", // Exponent
                    kid = "0f8e7d49-76ee-49d1-9706-156268e3ca03" // Identificador único de la clave
                }
            }
        };

        var jsonJwks = JsonSerializer.Serialize(jwks);
        return Result.Success(jsonJwks);
    }

    private User BuildUser(Dictionary<string, List<string>> claims, Dictionary<string, JsonElement> customData)
    {
        var roles = claims.GetValueOrDefault(ClaimTypes.Roles) ?? new List<string>();

        bool isInstructor = roles.Contains(ClaimTypes.RoleInstructor);
        bool isStudent = roles.Contains(ClaimTypes.RoleStudent);
        bool isWithoutRole = !isInstructor && !isStudent;

        return new User
        {
            Name = claims.GetValueOrDefault(ClaimTypes.Name)?.FirstOrDefault() ?? "Unknown",
            Email = claims.GetValueOrDefault(ClaimTypes.Email)?.FirstOrDefault() ?? "Not Provided",
            Picture = claims.GetValueOrDefault(ClaimTypes.Picture)?.FirstOrDefault() ?? "",
            UserId = customData.GetValueOrDefault(ClaimTypes.UserId).GetString() ?? "Unknown",
            IsInstructor = isInstructor,
            IsStudent = isStudent,
            IsWithoutRole = isWithoutRole
        };
    }

    private Course BuildCourse(Dictionary<string, List<string>> claims, Dictionary<string, JsonElement> customData)
    {
        var contextData = claims.GetValueOrDefault(ClaimTypes.Context)?.FirstOrDefault();

        var context = !string.IsNullOrEmpty(contextData)
            ? JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(contextData) ?? new()
            : new();

        return new Course
        {
            Id = customData.GetValueOrDefault(ClaimTypes.CourseId).GetString() ?? "Unknown",
            Label = context.GetValueOrDefault(ClaimTypes.Label).GetString() ?? "No Label",
            Title = context.GetValueOrDefault(ClaimTypes.Title).GetString() ?? "No Title",
            Type = context.GetValueOrDefault(ClaimTypes.Type).EnumerateArray()
                .Select(x => x.GetString() ?? "")
                .FirstOrDefault() ?? "Unknown"
        };
    }
}