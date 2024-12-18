using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using CSharpFunctionalExtensions;
using gec.Infrastructure.Lti.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace gec.Infrastructure.Lti;

public class LtiService : ILtiService
{
    private readonly string _urlBase;
    private readonly string _redirectUri;
    private readonly string _clientId;
    private readonly string _clientSecret;
    
    public LtiService(IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration), "La configuración no puede ser null.");
        }

        _urlBase = configuration["LtiSettings:UrlBase"] 
                   ?? throw new InvalidOperationException("LtiSettings:UrlBase no está configurado en la configuración.");
        _redirectUri = configuration["LtiSettings:RedirectUri"] 
                       ?? throw new InvalidOperationException("LtiSettings:RedirectUri no está configurado en la configuración.");
        _clientId = configuration["LtiSettings:ClientId"]
                       ?? throw new InvalidOperationException("LtiSettings:ClientId no está configurado en la configuración.");
        _clientSecret = configuration["LtiSettings:ClientSecret"]
                           ?? throw new InvalidOperationException("LtiSettings:ClientSecret no está configurado en la configuración.");
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
            $"redirect_uri={Uri.EscapeDataString(_redirectUri)}",
            $"login_hint={Uri.EscapeDataString(form.LoginHint)}",
            $"lti_message_hint={Uri.EscapeDataString(form.LtiMessageHint)}",
            $"state={Uri.EscapeDataString(state)}",
            $"response_mode=form_post",
            $"nonce={Uri.EscapeDataString(nonce)}",
            $"prompt=none"
        };

        var redirectUrl = $"{_urlBase}/api/lti/authorize_redirect?" + string.Join("&", queryParams);

        return Result.Success(redirectUrl);
    }

    public async Task<Result<ResourceContext>> HandleRedirectAsync(Dictionary<string, string> form)
    {
        var authenticationResponse = new AuthenticationResponse(form);
        var validate = authenticationResponse.Validate();
        if (validate.IsFailure) return Result.Failure<ResourceContext>(validate.Error);

        // Paso 2: Validar y decodificar el token
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = "https://canvas.test.instructure.com",
            ValidAudience = "143280000000000292", // Cambia a tu client_id
            IssuerSigningKeys =
                await GetSigningKeysFromJwksAsync("https://canvas.test.instructure.com/api/lti/security/jwks"),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        var handler = new JwtSecurityTokenHandler();
        try
        {
            // Validar el token
            handler.ValidateToken(authenticationResponse.IdToken, validationParameters, out var securityToken);

            // Decodificar información útil
            var jwtToken = (JwtSecurityToken)securityToken;
            var claims = jwtToken.Claims
                .GroupBy(c => c.Type)
                .ToDictionary(g => g.Key, g => g.Select(c => c.Value).ToList());

            // Extraer el claim "custom" y parsear los valores relevantes
            var customClaimJson = claims.ContainsKey("https://purl.imsglobal.org/spec/lti/claim/custom")
                ? claims["https://purl.imsglobal.org/spec/lti/claim/custom"].FirstOrDefault()
                : null;
            
            var customData = !string.IsNullOrEmpty(customClaimJson)
                ? JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(customClaimJson)
                : null;            

            var userId = customData != null && customData.ContainsKey("user_id")
                ? customData["user_id"].GetString() ?? "Desconocido"
                : "Desconocido";

            var courseId = customData != null && customData.ContainsKey("course_id")
                ? customData["course_id"].GetString() ?? "Desconocido"
                : "Desconocido";
            
            // Extraer datos del usuario
            var name = claims.ContainsKey("name") ? claims["name"].FirstOrDefault() : "Desconocido";
            var email = claims.ContainsKey("email") ? claims["email"].FirstOrDefault() : "No proporcionado";
            var picture = claims.ContainsKey("picture") ? claims["picture"].FirstOrDefault() : null;
            var roles = claims.ContainsKey("https://purl.imsglobal.org/spec/lti/claim/roles")
                ? claims["https://purl.imsglobal.org/spec/lti/claim/roles"]
                : new List<string> { "Sin roles" };

            // Crear objetos para usuario y curso
            var user = new User()
            {
                Name = name ?? string.Empty,
                Email = email ?? string.Empty,
                UserId = userId ?? string.Empty,
                Roles = roles,
                Picture = picture ?? string.Empty
            };

            // Extraer datos del curso
            var course = claims.ContainsKey("https://purl.imsglobal.org/spec/lti/claim/context")
                ? JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                    claims["https://purl.imsglobal.org/spec/lti/claim/context"].FirstOrDefault() ?? "{}")
                : null;

            var courseData = new Course()
            {
                Id = courseId,
                Label = course != null && course.ContainsKey("label") && course["label"].GetString() != null 
                    ? course["label"].GetString()! 
                    : "Sin etiqueta",
                Title = course != null && course.ContainsKey("title") && course["title"].GetString() != null
                    ? course["title"].GetString()! 
                    : "Sin título",
                Description = (course != null && course.ContainsKey("description") 
                    ? course["description"].GetString() 
                    : null) ?? string.Empty,
                Type = course != null && course.ContainsKey("type") && course["type"].ValueKind == JsonValueKind.Array
                    ? course["type"].EnumerateArray().Select(x => x.GetString() ?? "").ToList()
                    : new List<string>()
            };            

            return Result.Success(new ResourceContext() { User = user, Course = courseData });
        }
        catch (SecurityTokenException ex)
        {
            return Result.Failure<ResourceContext>($"El token es inválido: {ex.Message}");
        }
    }

    private async Task<IEnumerable<SecurityKey>> GetSigningKeysFromJwksAsync(string jwksUrl)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync(jwksUrl);

        var jwks = new JsonWebKeySet(response);
        return jwks.Keys;
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
    
    public async Task<Result<string>> GetUserAccessTokenAsync(int userId)
    {
        var tokenEndpoint = $"{_urlBase}/login/oauth2/token";


        // Construir el cuerpo de la solicitud
        var requestData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", _clientSecret)
        });

        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(tokenEndpoint, requestData);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return Result.Failure<string>($"Error en la solicitud del token: {response.StatusCode} - {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return Result.Success(tokenResponse.AccessToken);
    }
    
}