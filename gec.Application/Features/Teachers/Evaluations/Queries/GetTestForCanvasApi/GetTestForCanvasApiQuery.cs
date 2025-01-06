using CSharpFunctionalExtensions;
using MediatR;

namespace gec.Application.Features.Teachers.Evaluations.Queries.GetTestForCanvasApi;

public class GetTestForCanvasApiQuery : IRequest<Result<GetTestForCanvasApiRespond>>
{
    public string CourseId { get; set; } = string.Empty;
}