using System.Text.Json;
using gec.Infrastructure.Lti;
using gec.Infrastructure.Lti.Models;
using gec.Server.Common;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Controllers;

[ApiController]
public class LtiController : BaseController
{
    private readonly ILogger<LtiController> _logger;
    private readonly ILtiService _ltiService;

    public LtiController(ILogger<LtiController> logger, ILtiService ltiService)
    {
        _logger = logger;
        _ltiService = ltiService;
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
        var formModel = new AuthenticationResponse(form.ToDictionary(x => x.Key, x => x.Value.ToString()));
        var context = await _ltiService.HandleRedirectAsync(formModel);
        if (context.IsFailure) return Error(context.Error);
        
        HttpContext.Session.SetString("resourceContext", JsonSerializer.Serialize(context.Value));
        return Redirect("/");
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
}
