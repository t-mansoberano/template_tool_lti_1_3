using CSharpFunctionalExtensions;
using MediatR;

namespace gec.Application.Features.Teachers.Evaluations.Queries.GetTestForCanvasAPI;

public class GetTestForCanvasAPIQuery : IRequest<Result<GetTestForCanvasAPIRespond>>
{
}