using CSharpFunctionalExtensions;
using MediatR;

namespace gec.Application.Features.Instructors.Evaluations.Queries.GetStudentCourseEvaluationsView;

public class GetStudentCourseEvaluationsViewQuery : IRequest<Result<GetStudentCourseEvaluationsViewRespond>>
{
    public string CourseId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}