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
        if (validate.IsFailure) return validate;

        var state = Guid.NewGuid().ToString();
        var nonce = Guid.NewGuid().ToString();

        var queryParams = new List<string>
        {
            $"scope=openid",
            $"response_type=id_token",
            $"client_id={form.ClientId}",
            $"redirect_uri={Uri.EscapeDataString(_appSettings.LtiRedirectUri)}",
            $"login_hint={Uri.EscapeDataString(form.LoginHint)}",
            $"lti_message_hint={Uri.EscapeDataString(form.LtiMessageHint)}",
            $"state={Uri.EscapeDataString(state)}",
            $"response_mode=form_post",
            $"nonce={Uri.EscapeDataString(nonce)}",
            $"prompt=none"
        };

        var redirectUrl = $"{_appSettings.LtiUrlBase}/api/lti/authorize_redirect?" + string.Join("&", queryParams);
        return Result.Success(redirectUrl);
    }

    public async Task<Result<LtiContext>> HandleRedirectAsync(Dictionary<string, string> form)
    {
        var authenticationResponse = new AuthenticationResponse(form);
        var validate = authenticationResponse.Validate();
        if (validate.IsFailure) return Result.Failure<LtiContext>(validate.Error);

        var result = await _jwtValidationService.ValidateTokenAsync(authenticationResponse.IdToken);
        if (result.IsFailure) return Result.Failure<LtiContext>(result.Error);

        var claims = result.Value.Claims.GroupBy(c => c.Type)
            .ToDictionary(g => g.Key, g => g.Select(v => v.Value).ToList());

        // Extraer y deserializar el claim "custom"
        var customClaimJson = claims.ContainsKey("https://purl.imsglobal.org/spec/lti/claim/custom")
            ? claims["https://purl.imsglobal.org/spec/lti/claim/custom"].FirstOrDefault()
            : null;

        var customData = !string.IsNullOrEmpty(customClaimJson)
            ? JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(customClaimJson) ??
              new Dictionary<string, JsonElement>()
            : new Dictionary<string, JsonElement>();

        var user = ParseUser(claims, customData);
        var course = ParseCourse(claims, customData);

        return Result.Success(new LtiContext { User = user, Course = course });
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

    private User ParseUser(Dictionary<string, List<string>> claims, Dictionary<string, JsonElement> customData)
    {
        var userId = customData != null && customData.ContainsKey("user_id")
            ? customData["user_id"].GetString() ?? "Desconocido"
            : "Desconocido";

        return new User
        {
            Name = claims.ContainsKey("name") ? claims["name"].FirstOrDefault() ?? "Desconocido" : "Desconocido",
            Email = claims.ContainsKey("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
                ? claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"].FirstOrDefault() ??
                  "No proporcionado"
                : "No proporcionado",
            Picture = claims.ContainsKey("picture") ? claims["picture"].FirstOrDefault() ?? "" : "",
            Roles = claims.ContainsKey("https://purl.imsglobal.org/spec/lti/claim/roles")
                ? claims["https://purl.imsglobal.org/spec/lti/claim/roles"]
                : new List<string> { "Sin roles" },
            UserId = userId
        };
    }

    private Course ParseCourse(Dictionary<string, List<string>> claims, Dictionary<string, JsonElement> customData)
    {
        var courseId = customData != null && customData.ContainsKey("course_id")
            ? customData["course_id"].GetString() ?? "Desconocido"
            : "Desconocido";

        var contextData = claims.ContainsKey("https://purl.imsglobal.org/spec/lti/claim/context")
            ? JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                claims["https://purl.imsglobal.org/spec/lti/claim/context"].FirstOrDefault() ?? "{}")
            : null;

        return new Course
        {
            Id = courseId,
            Label = contextData != null && contextData.ContainsKey("label")
                ? contextData["label"].GetString() ?? "Sin etiqueta"
                : "Sin etiqueta",
            Title = contextData != null && contextData.ContainsKey("title")
                ? contextData["title"].GetString() ?? "Sin título"
                : "Sin título",
            Description = contextData != null && contextData.ContainsKey("description")
                ? contextData["description"].GetString() ?? string.Empty
                : string.Empty,
            Type = contextData != null && contextData.ContainsKey("type") &&
                   contextData["type"].ValueKind == JsonValueKind.Array
                ? contextData["type"].EnumerateArray().Select(x => x.GetString() ?? "").ToList()
                : new List<string>()
        };
    }
}