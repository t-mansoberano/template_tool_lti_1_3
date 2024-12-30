using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments;
using MediatR;

namespace gec.Application.Features.Teachers.Evaluations.Queries.GetTestForCanvasAPI;

public class GetTestForCanvasAPIHandle : IRequestHandler<GetTestForCanvasAPIQuery, Result<GetTestForCanvasAPIRespond>>
{
    private readonly IEnrollmentsService _enrollmentsService;

    public GetTestForCanvasAPIHandle(IEnrollmentsService enrollmentsService)
    {
        _enrollmentsService = enrollmentsService;
    }

    public async Task<Result<GetTestForCanvasAPIRespond>> Handle(GetTestForCanvasAPIQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _enrollmentsService.GetStudentsByCourseAsync();

        var resultCoverted = result.Value.Select(s => new Enrollment()
        {
            Id = s.Id, Type = s.Type, UserId = s.UserId, CourseId = s.CourseId,
            Grades = new Grades()
            {
                CurrentGrade = s.Grades.CurrentGrade, FinalGrade = s.Grades.FinalGrade,
                CurrentScore = s.Grades.CurrentScore, HtmlUrl = s.Grades.HtmlUrl, FinalScore = s.Grades.FinalScore
            },
            User = new User() { Id = s.User.Id, Name = s.User.Name }
        }).ToList(); 
        
        var resultFinal = new GetTestForCanvasAPIRespond() { Enrollments = resultCoverted };
        
        return Result.Success(resultFinal);
    }
}