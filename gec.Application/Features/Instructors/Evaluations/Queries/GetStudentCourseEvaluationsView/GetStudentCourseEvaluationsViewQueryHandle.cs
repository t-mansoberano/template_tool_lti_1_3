using CSharpFunctionalExtensions;
using gec.Application.Common;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments;
using gec.Application.Contracts.Infrastructure.Canvas.Submissions;
using gec.Application.Contracts.Infrastructure.Canvas.Submissions.Models;
using gec.Application.Contracts.Server.Session;
using gec.Application.Features.Instructors.Evaluations.Dto;
using MediatR;

namespace gec.Application.Features.Instructors.Evaluations.Queries.GetStudentCourseEvaluationsView;

public class GetStudentCourseEvaluationsViewQueryHandle : IRequestHandler<GetStudentCourseEvaluationsViewQuery,
    Result<GetStudentCourseEvaluationsViewRespond>>
{
    private readonly IMapper<Submission, StudentEvidences> _canvasSubmissionStudentEvidenceMapper;
    private readonly ISubmissionsService _submissionsService;
    private readonly IEnrollmentsService _enrollmentsService;

    public GetStudentCourseEvaluationsViewQueryHandle(ISessionStorageService sessionStorageService,
        IMapper<Submission, StudentEvidences> canvasSubmissionStudentEvidenceMapper,
        ISubmissionsService submissionsService,
        IEnrollmentsService enrollmentsService)
    {
        _canvasSubmissionStudentEvidenceMapper = canvasSubmissionStudentEvidenceMapper;
        _submissionsService = submissionsService;
        _enrollmentsService = enrollmentsService;
    }

    public async Task<Result<GetStudentCourseEvaluationsViewRespond>> Handle(
        GetStudentCourseEvaluationsViewQuery request,
        CancellationToken cancellationToken)
    {
        var instructorsResult = await _enrollmentsService.GetInstructorsByCourseAsync(request.CourseId);
        if (instructorsResult.IsFailure)
            return Result.Failure<GetStudentCourseEvaluationsViewRespond>(instructorsResult.Error);

        var instructorIds = instructorsResult.Value.Select(e => e.UserId).ToList();

        var submissionsResult =
            await _submissionsService.GetSubmissionsByStudentAsync(request.CourseId, request.UserId);
        if (submissionsResult.IsFailure)
            return Result.Failure<GetStudentCourseEvaluationsViewRespond>(submissionsResult.Error);

        var studentsEvidences =
            _canvasSubmissionStudentEvidenceMapper.MapWithDependencies(submissionsResult.Value, instructorIds);

        var response = new GetStudentCourseEvaluationsViewRespond
        {
            StudentEvidences = studentsEvidences
        };

        return response;
    }
}