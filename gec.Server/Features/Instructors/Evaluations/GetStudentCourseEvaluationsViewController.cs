using gec.Application.Contracts.Server.Configuration;
using gec.Application.Contracts.Server.Fake;
using gec.Application.Features.Instructors.Evaluations.Queries.GetStudentCourseEvaluationsView;
using gec.Server.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Features.Instructors.Evaluations;

[ApiController]
[Route("api/teachers/courses/{courseId}/students/{studentId}/evaluations")]
[TypeFilter(typeof(ValidateCanvasTokenAttribute))]
public class GetStudentCourseEvaluationsViewController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IFakeDataService _fakeDataService;
    private readonly IAppSettingsService _appSettingsService;

    public GetStudentCourseEvaluationsViewController(IMediator mediator, IFakeDataService fakeDataService,
        IAppSettingsService appSettingsService)
    {
        _mediator = mediator;
        _fakeDataService = fakeDataService;
        _appSettingsService = appSettingsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentSubmissions([FromRoute] string courseId, [FromRoute] string studentId)
    {
        if (_appSettingsService.Fake.UseFakeApiCanvas)
            return Ok(_fakeDataService
                .GetFakeData<dynamic>(_appSettingsService.Fake.FakeStudentCourseEvaluationsViewPath).Value);

        var query = new GetStudentCourseEvaluationsViewQuery() { CourseId = courseId, UserId = studentId };
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}