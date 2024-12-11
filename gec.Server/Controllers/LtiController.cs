using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json;
using gec.Server.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;

namespace gec.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LtiController : BaseController
{
    private readonly ILogger<LtiController> _logger;
    private readonly HttpClient _httpClient;

    public LtiController(ILogger<LtiController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userJson = HttpContext.Session.GetString("user");
        var courseJson = HttpContext.Session.GetString("course");

        if (string.IsNullOrEmpty(userJson) || string.IsNullOrEmpty(courseJson))
        {
            return BadRequest("No hay datos guardados en la sesión.");
        }

        var user = JsonSerializer.Deserialize<UserDto>(userJson);
        var course = JsonSerializer.Deserialize<CourseDto>(courseJson);

        return Ok(new { user, course });
    }

    [HttpPost]
    public IActionResult LaunchLTI([FromForm] IFormCollection form)
    {
        // Validar que los parámetros necesarios están presentes
        if (!form.TryGetValue("iss", out var issuer) ||
            !form.TryGetValue("login_hint", out var loginHint) ||
            !form.TryGetValue("lti_message_hint", out var ltiMessageHint) ||
            !form.TryGetValue("client_id", out var clientId) ||
            !form.TryGetValue("target_link_uri", out var targetLinkUri))
        {
            return BadRequest("Faltan parámetros requeridos para el OIDC Launch.");
        }

        var state = Guid.NewGuid().ToString();
        HttpContext.Session.SetString("oidc_state", state); // Guardar el estado

        var nonce = Guid.NewGuid().ToString();
        HttpContext.Session.SetString("oidc_nonce", nonce); // Guardar el nonce

        var redirectUri = Url.Action("HandleRedirect", "Lti", null, Request.Scheme);
        var urlBase = "https://sso.test.canvaslms.com";

        var redirectUrl = QueryHelpers.AddQueryString($"{urlBase}/api/lti/authorize_redirect", new Dictionary<string, string>
        {
            { "scope", "openid" },
            { "response_type", "id_token" },
            { "client_id", clientId },
            { "redirect_uri", "https://manuelsoberano.ngrok.dev/api/lti/redirect" },
            { "login_hint", loginHint },
            { "lti_message_hint", ltiMessageHint },
            { "state", state },
            { "response_mode", "form_post" },
            { "nonce", nonce },
            { "prompt", "none" } 
        });
        
        return Redirect(redirectUrl);
    }
    
    [HttpPost]
    [Route("redirect")]
    public async Task<IActionResult> HandleRedirect([FromForm] IFormCollection form)
    {
        // Paso 1: Obtener el `id_token` del formulario enviado por Canvas
        if (!form.TryGetValue("id_token", out var idToken))
        {
            return BadRequest("El id_token no está presente en la solicitud.");
        }
    
        // Paso 2: Validar y decodificar el token
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = "https://canvas.test.instructure.com",
            ValidAudience = "143280000000000292", // Cambia a tu client_id
            IssuerSigningKeys = await GetSigningKeysFromJWKSAsync("https://canvas.test.instructure.com/api/lti/security/jwks"),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
        
        var handler = new JwtSecurityTokenHandler();
        try
        {
            // Validar el token
            var principal = handler.ValidateToken(idToken, validationParameters, out var securityToken);
    
            // Decodificar información útil
            var jwtToken = (JwtSecurityToken)securityToken;
            var claims = jwtToken.Claims
                .GroupBy(c => c.Type)
                .ToDictionary(g => g.Key, g => g.Select(c => c.Value).ToList());
    
            // Extraer datos del usuario
            var name = claims.ContainsKey("name") ? claims["name"].FirstOrDefault() : "Desconocido";
            var email = claims.ContainsKey("email") ? claims["email"].FirstOrDefault() : "No proporcionado";
            var picture = claims.ContainsKey("picture") ? claims["picture"].FirstOrDefault() : null;
            var roles = claims.ContainsKey("https://purl.imsglobal.org/spec/lti/claim/roles")
                ? claims["https://purl.imsglobal.org/spec/lti/claim/roles"]
                : new List<string> { "Sin roles" };

            // Crear objetos para usuario y curso
            var user = new UserDto()
            {
                Name = name,
                Email = email,
                Roles = roles,
                Picture = picture
            };

            // Extraer datos del curso
            var course = claims.ContainsKey("https://purl.imsglobal.org/spec/lti/claim/context")
                ? JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(claims["https://purl.imsglobal.org/spec/lti/claim/context"].FirstOrDefault() ?? "{}")
                : null;

            var courseData = new CourseDto()
            {
                Id = course != null && course.ContainsKey("id") ? course["id"].GetString() : "Desconocido",
                Label = course != null && course.ContainsKey("label") ? course["label"].GetString() : "Sin etiqueta",
                Title = course != null && course.ContainsKey("title") ? course["title"].GetString() : "Sin título",
                Description = course != null && course.ContainsKey("description") ? course["description"].GetString() : null,
                Type = course != null && course.ContainsKey("type") && course["type"].ValueKind == JsonValueKind.Array
                    ? course["type"].EnumerateArray().Select(x => x.GetString()).ToList()
                    : new List<string>()
            };

            // Guardar datos relevantes en la sesión
            HttpContext.Session.SetString("user", JsonSerializer.Serialize(user));
            HttpContext.Session.SetString("course", JsonSerializer.Serialize(courseData));

            return Redirect("/");
        }
        catch (SecurityTokenException ex)
        {
            return Unauthorized(new { Error = "El token es inválido.", Details = ex.Message });
        }
    }
    
    private async Task<IEnumerable<SecurityKey>> GetSigningKeysFromJWKSAsync(string jwksUrl)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync(jwksUrl);

        var jwks = new JsonWebKeySet(response);
        return jwks.Keys;
    }
}

public class UserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
    public string Picture { get; set; }
}

public class CourseDto
{
    public string Id { get; set; }
    public string Label { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<string> Type { get; set; }
}