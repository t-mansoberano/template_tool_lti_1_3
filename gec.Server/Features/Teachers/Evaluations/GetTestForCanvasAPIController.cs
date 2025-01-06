using gec.Application.Features.Teachers.Evaluations.Queries.GetTestForCanvasApi;
using gec.Server.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Features.Teachers.Evaluations;

[ApiController]
[Route("api/teachers/courses/{courseId}/testapicanvas")]
[TypeFilter(typeof(ValidateCanvasTokenAttribute))]
public class GetTestForCanvasApiController : BaseController
{
    private readonly IMediator _mediator;

    public GetTestForCanvasApiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentsByCourseAsync([FromRoute] string courseId,
        [FromQuery] GetTestForCanvasApiQuery query)
    {
        query.CourseId = courseId;
        var result = await _mediator.Send(query);
        if (result.IsFailure)
            return Error(result.Error);

        return Ok(result.Value);
    }
}