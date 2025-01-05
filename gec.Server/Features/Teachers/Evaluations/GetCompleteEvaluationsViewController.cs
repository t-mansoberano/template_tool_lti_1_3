using gec.Application.Features.Teachers.Evaluations.Queries.GetCompleteEvaluationsView;
using gec.Server.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Features.Teachers.Evaluations;

[ApiController]
[Route("api/teachers/courses/{courseId}/evaluations")]
[TypeFilter(typeof(ValidateCanvasTokenAttribute))]
public class GetCompleteEvaluationsViewController : BaseController
{
    private readonly IMediator _mediator;

    public GetCompleteEvaluationsViewController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentsByCourseAsync([FromQuery] GetCompleteEvaluationsViewQuery query)
    {
        var result = await _mediator.Send(query);
        if (result.IsFailure) 
            return Error(result.Error);

        return Ok(result.Value);
    }
}