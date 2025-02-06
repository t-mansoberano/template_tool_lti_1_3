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

    public GetStudentCourseEvaluationsViewController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentSubmissions([FromRoute] string courseId, [FromRoute] string studentId)
    {
        var query = new GetStudentCourseEvaluationsViewQuery() { CourseId = courseId, UserId = studentId };
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}