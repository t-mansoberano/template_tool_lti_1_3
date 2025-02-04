using CSharpFunctionalExtensions;
using MediatR;

namespace gec.Application.Features.Instructors.Evaluations.Queries.GetCompleteEvaluationsView;

public class GetCompleteEvaluationsViewQuery : IRequest<Result<GetCompleteEvaluationsViewRespond>>
{
    public string CourseId { get; set; } = string.Empty;
}