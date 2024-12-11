using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Controllers;

[ApiController]
[Route("api/.well-known/jwks.json")]
public class JwksController : ControllerBase
{
    [HttpGet]
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
                    n = "wwo9Um22g6yojCGpqgk-I7KlEyUExHP8iHr6rrBqgEX8QD99nEtjtgEDb2Dc3xWkcMj1E57K7SAym5-7HXBu7b6dURwjv3KJk_FxuSDK43MNKiJsn1IOuToXJwcE-O6Jz67zOAL4Vnz-s1mIsLSWUkYeVg4l9ixfj3Ddo37TWn75WQbN2TiFGQzPaJXDeBHDOhvwClCCAXnBcS0PlLrujyqAvyNOdqv-7oMlaTdLxyI1RNLLHMDNxjohadJlEd77n-p6id34RLGaFeTW5hj8DhcCNrE9FmysOCdZy9Fj2DLVu0FvKfqd-X3NN-jpZZO1uEZd-q8GzqXqHQcACNFiRw", // Modulus
                    e = "AQAB", // Exponent
                    kid = "0f8e7d49-76ee-49d1-9706-156268e3ca03" // Identificador único de la clave
                }
            }
        };

        // Devuelve el JSON con la clave pública
        return Ok(jwks);
    }
}
