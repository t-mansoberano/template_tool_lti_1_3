using System.Text.Json;
using System.Text.Json.Serialization;
using gec.Infrastructure.Lti;
using gec.Infrastructure.Lti.Models;
using gec.Server.Common;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Controllers.Lti;

[ApiController]
public class LtiController : BaseController
{
    private readonly ILtiService _ltiService;
    private readonly IConfiguration _configuration;

    public LtiController(ILtiService ltiService, IConfiguration configuration)
    {
        _ltiService = ltiService;
        _configuration = configuration;
    }

    [HttpGet]
    [Route("api/lti")]
    public Task<IActionResult> Get()
    {
        var resourceContextJson = HttpContext.Session.GetString("resourceContext");

        if (string.IsNullOrEmpty(resourceContextJson))
        {
            return Task.FromResult(Error("No hay datos guardados en la sesión."));
        }

        var resourceContext = JsonSerializer.Deserialize<ResourceContext>(resourceContextJson);

        return Task.FromResult(Ok(resourceContext));
    }
    
    [HttpPost]
    [Route("api/lti")]
    public IActionResult LaunchLTI([FromForm] IFormCollection form)
    {
        var formModel = new LoginInitiationResponse(form.ToDictionary(x => x.Key, x => x.Value.ToString()));
        var redirectUrl = _ltiService.BuildAuthorizationUrl(formModel);
        if (redirectUrl.IsFailure) return Error(redirectUrl.Error);
        
        return Redirect(redirectUrl.Value);
    }

    [HttpPost]
    [Route("api/lti/redirect")]
    public async Task<IActionResult> HandleRedirect([FromForm] IFormCollection form)
    {
        var context = await _ltiService.HandleRedirectAsync(form.ToDictionary(x => x.Key, x => x.Value.ToString()));
        if (context.IsFailure) return Error(context.Error);
        
        HttpContext.Session.SetString("resourceContext", JsonSerializer.Serialize(context.Value));

        return Redirect("/api/lti/oauth/authorize");        
    }
    
    [HttpGet]
    [Route("api/.well-known/jwks.json")]
    public IActionResult GetJwks()
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

        // Devuelve el JSON con la clave pública
        return Ok(jwks);
    }

    // [HttpGet]
    // [Route("api/lti/oauth/authorize")]
    // public IActionResult AuthorizeUser()
    // {
    //     var clientId = _configuration["Canvas:ClientId"];
    //     var redirectUri = _configuration["Canvas:RedirectUri"];
    //     var canvasBaseUrl = _configuration["Canvas:ApiBaseUrl"];
    //
    //     var authorizationUrl = $"{canvasBaseUrl}/login/oauth2/auth?" +
    //                            $"client_id={clientId}&" +
    //                            $"response_type=code&" +
    //                            $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
    //                            $"scope=url:GET|/api/v1/courses/:id url:GET|/api/v1/courses/:course_id/enrollments url:GET|/api/v1/courses/:course_id/folders url:GET|/api/v1/folders/:id/folders url:GET|/api/v1/folders/:id/files url:GET|/api/v1/courses/:course_id/assignments url:GET|/api/v1/courses/:course_id/assignments/:assignment_id/submissions/:user_id url:GET|/api/v1/announcements url:GET|/api/v1/users/self/favorites/courses url:GET|/api/v1/users/:id url:GET|/api/v1/courses url:GET|/api/v1/accounts/:account_id/sub_accounts url:GET|/api/v1/courses/:course_id/todo url:GET|/api/v1/users/:user_id/courses url:GET|/api/v1/courses/:id&" +
    //                            $"&state={Guid.NewGuid()}";
    //
    //     return Redirect(authorizationUrl);
    // }
    //
    // [HttpGet]
    // [Route("api/lti/oauth/callback")]
    // public async Task<IActionResult> HandleOAuthCallback([FromQuery] string code, [FromQuery] string state)
    // {
    //     // 1. Validar que el parámetro "code" existe
    //     if (string.IsNullOrEmpty(code))
    //     {
    //         return Error("No se recibió el código de autorización.");
    //     }
    //
    //     // 2. Configurar parámetros para el Token Endpoint
    //     var tokenEndpoint = $"{_configuration["Canvas:ApiBaseUrl"]}/login/oauth2/token";
    //     var clientId = _configuration["Canvas:ClientId"];
    //     var clientSecret = _configuration["Canvas:ClientSecret"];
    //     var redirectUri = _configuration["Canvas:RedirectUri"];
    //
    //     var requestData = new FormUrlEncodedContent(new[]
    //     {
    //         new KeyValuePair<string, string>("grant_type", "authorization_code"),
    //         new KeyValuePair<string, string>("client_id", clientId),
    //         new KeyValuePair<string, string>("client_secret", clientSecret),
    //         new KeyValuePair<string, string>("redirect_uri", redirectUri),
    //         new KeyValuePair<string, string>("code", code)
    //     });
    //
    //     // 3. Realizar la solicitud HTTP POST al Token Endpoint
    //     using var httpClient = new HttpClient();
    //     var response = await httpClient.PostAsync(tokenEndpoint, requestData);
    //
    //     // 4. Manejar la respuesta
    //     if (!response.IsSuccessStatusCode)
    //     {
    //         var errorContent = await response.Content.ReadAsStringAsync();
    //         return Error($"Error al obtener el token: {errorContent}");
    //     }
    //
    //     var content = await response.Content.ReadAsStringAsync();
    //     var tokenResponse = JsonSerializer.Deserialize<TokenResponse2>(content, new JsonSerializerOptions
    //     {
    //         PropertyNameCaseInsensitive = true
    //     });
    //
    //     // 5. Almacenar el token en sesión o base de datos
    //     HttpContext.Session.SetString("tokenResponse", JsonSerializer.Serialize(tokenResponse));
    //
    //     return Redirect("/");
    // }
    
}

public class TokenResponse2
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonPropertyName("user")]
    public UserInfo User { get; set; }

    [JsonPropertyName("canvas_region")]
    public string CanvasRegion { get; set; }
}

public class UserInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("global_id")]
    public string GlobalId { get; set; }

    [JsonPropertyName("effective_locale")]
    public string EffectiveLocale { get; set; }
}
