using CSharpFunctionalExtensions;
using gec.Application.Common;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments;
using MediatR;

namespace gec.Application.Features.Instructors.Evaluations.Queries.GetTestForCanvasApi;

public class GetTestForCanvasApiHandle : IRequestHandler<GetTestForCanvasApiQuery, Result<GetTestForCanvasApiRespond>>
{
    private readonly IEnrollmentsService _enrollmentsService;

    public GetTestForCanvasApiHandle(IEnrollmentsService enrollmentsService)
    {
        _enrollmentsService = enrollmentsService;
    }

    public async Task<Result<GetTestForCanvasApiRespond>> Handle(GetTestForCanvasApiQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await new GetTestForCanvasApiQueryValidator().ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Failure<GetTestForCanvasApiRespond>(validationResult.ErrorMessages());

        var result = await _enrollmentsService.GetStudentsByCourseAsync(request.CourseId);

        var resultCoverted = result.Value.Select(s => new Enrollment
        {
            Id = s.Id, Type = s.Type, UserId = s.UserId, CourseId = s.CourseId,
            Grades = new Grades
            {
                CurrentGrade = s.Grades.CurrentGrade, FinalGrade = s.Grades.FinalGrade,
                CurrentScore = s.Grades.CurrentScore, HtmlUrl = s.Grades.HtmlUrl, FinalScore = s.Grades.FinalScore
            },
            User = new User { Id = s.User.Id, Name = s.User.Name }
        }).ToList();

        var resultFinal = new GetTestForCanvasApiRespond { Enrollments = resultCoverted };

        return Result.Success(resultFinal);
    }
}