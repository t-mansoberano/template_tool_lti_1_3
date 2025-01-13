using gec.Application.Contracts.Infrastructure.Federation;
using gec.Server.Common;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Integrations.Federation;

[ApiController]
public class FederationAuthController : BaseController
{
    private readonly IFederationService _federationService;

    public FederationAuthController(IFederationService federationService)
    {
        _federationService = federationService;
    }

    [HttpGet]
    [Route("api/federation")]
    public async Task<IActionResult> Get()
    {
        var federationContext = await _federationService.HandleAuthAsync();
        if (federationContext.IsFailure)
            return Error(federationContext.Error);

        return Ok(federationContext.Value);
    }
}