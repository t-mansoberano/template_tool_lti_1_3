using gec.Application.Contracts.Infrastructure.Lti;
using gec.Application.Contracts.Server.Session;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Integrations.Lti;

[ApiController]
public class JwksController : ControllerBase
{
    private readonly ILtiService _ltiService;

    public JwksController(ILtiService ltiService, ISessionStorageService sessionStorageService)
    {
        _ltiService = ltiService;
    }
    [HttpGet]
    [Route("/api/.well-known/jwks.json")]
    public IActionResult GetJwks()
    {
        return Ok(_ltiService.GetJwks().Value);
    }
}