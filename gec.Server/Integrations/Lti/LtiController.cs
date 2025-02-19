using gec.Application.Contracts.Infrastructure.Lti;
using gec.Application.Contracts.Infrastructure.Lti.Models;
using gec.Application.Contracts.Server.Configuration;
using gec.Application.Contracts.Server.Fake;
using gec.Application.Contracts.Server.Session;
using gec.Server.Common;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Integrations.Lti;

[ApiController]
[Route("api/lti")]
public class LtiController : BaseController
{
    private readonly ILtiService _ltiService;
    private readonly ISessionStorageService _sessionStorageService;
    private readonly IAppSettingsService _appSettingsService;
    private readonly IFakeDataService _fakeDataService;

    public LtiController(ILtiService ltiService, ISessionStorageService sessionStorageService,
        IAppSettingsService appSettingsService, IFakeDataService fakeDataService)
    {
        _ltiService = ltiService;
        _sessionStorageService = sessionStorageService;
        _appSettingsService = appSettingsService;
        _fakeDataService = fakeDataService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        if (_appSettingsService.Fake.UseFakeLti) 
            return Ok(_fakeDataService.GetFakeData<LtiContext>(_appSettingsService.Fake.FakeLtiContextPath).Value);

        var ltiContext = _sessionStorageService.Retrieve<LtiContext>(LtiContext.Key);
        if (ltiContext.IsFailure)
            return Error(ltiContext.Error);

        return Ok(ltiContext.Value);
    }

    [HttpPost]
    public IActionResult LaunchLTI([FromForm] IFormCollection form)
    {
        var formModel = new LoginInitiationResponse(form.ToDictionary(x => x.Key, x => x.Value.ToString()));
        var redirectUrl = _ltiService.BuildAuthorizationUrl(formModel);
        if (redirectUrl.IsFailure)
            return Error(redirectUrl.Error);

        return Redirect(redirectUrl.Value);
    }

    [HttpPost]
    [Route("redirect")]
    public async Task<IActionResult> HandleRedirect([FromForm] IFormCollection form)
    {
        var context = await _ltiService.HandleRedirectAsync(form.ToDictionary(x => x.Key, x => x.Value.ToString()));
        if (context.IsFailure)
            return Error(context.Error);

        _sessionStorageService.Store(LtiContext.Key, context.Value);

        return Redirect("/api/lti/oauth/token/validate");
    }
}