using gec.Application.Contracts.Server.Configuration;
using gec.Application.Contracts.Server.Fake;
using gec.Application.Features.Instructors.Evaluations.Queries.GetCompleteEvaluationsView;
using gec.Server.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Features.Instructors.Evaluations;

[ApiController]
[Route("api/teachers/courses/{courseId}/evaluations")]
[TypeFilter(typeof(ValidateCanvasTokenAttribute))]
public class GetCompleteEvaluationsViewController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IFakeDataService _fakeDataService;
    private readonly IAppSettingsService _appSettingsService;

    public GetCompleteEvaluationsViewController(IMediator mediator, IFakeDataService fakeDataService,
        IAppSettingsService appSettingsService)
    {
        _mediator = mediator;
        _fakeDataService = fakeDataService;
        _appSettingsService = appSettingsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentsByCourseAsync([FromRoute] string courseId,
        [FromQuery] GetCompleteEvaluationsViewQuery query)
    {
        if (_appSettingsService.Fake.UseFakeApiCanvas)
            return Ok(_fakeDataService.GetFakeData<dynamic>(_appSettingsService.Fake.FakeCompleteEvaluationsViewPath)
                .Value);

        query.CourseId = courseId;
        var result = await _mediator.Send(query);
        if (result.IsFailure)
            return Error(result.Error);

        return Ok(result.Value);
    }
}