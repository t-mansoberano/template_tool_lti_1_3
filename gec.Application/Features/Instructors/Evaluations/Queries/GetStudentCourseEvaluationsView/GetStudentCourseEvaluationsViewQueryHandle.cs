using CSharpFunctionalExtensions;
using gec.Application.Common;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments;
using gec.Application.Contracts.Infrastructure.Canvas.Submissions;
using gec.Application.Contracts.Infrastructure.Canvas.Submissions.Models;
using gec.Application.Contracts.Infrastructure.Database.GetStudentEvaluations;
using gec.Application.Contracts.Infrastructure.Database.GetStudentEvaluations.Models;
using gec.Application.Contracts.Server.Session;
using gec.Application.Features.Instructors.Evaluations.Dto;
using MediatR;

namespace gec.Application.Features.Instructors.Evaluations.Queries.GetStudentCourseEvaluationsView;

public class GetStudentCourseEvaluationsViewQueryHandle : IRequestHandler<GetStudentCourseEvaluationsViewQuery,
    Result<GetStudentCourseEvaluationsViewRespond>>
{
    private readonly IMapper<Submission, StudentEvidences> _canvasSubmissionStudentEvidenceMapper;
    private readonly IMapper<StudentEvaluationResult, StudentEvaluationResults> _studentEvaluationResultMapper;
    private readonly ISubmissionsService _submissionsService;
    private readonly IEnrollmentsService _enrollmentsService;
    private readonly IGetStudentEvaluationsRepository _getStudentEvaluationsRepository;

    public GetStudentCourseEvaluationsViewQueryHandle(ISessionStorageService sessionStorageService,
        IMapper<Submission, StudentEvidences> canvasSubmissionStudentEvidenceMapper,
        IMapper<StudentEvaluationResult, StudentEvaluationResults> studentEvaluationResultMapper,
        ISubmissionsService submissionsService,
        IEnrollmentsService enrollmentsService,
        IGetStudentEvaluationsRepository getStudentEvaluationsRepository)
    {
        _getStudentEvaluationsRepository = getStudentEvaluationsRepository;
        _canvasSubmissionStudentEvidenceMapper = canvasSubmissionStudentEvidenceMapper;
        _studentEvaluationResultMapper = studentEvaluationResultMapper;
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

        var studentEvaluationResults = await _getStudentEvaluationsRepository.Get(request.CourseId, request.UserId);
        if (studentEvaluationResults.IsFailure)
            return Result.Failure<GetStudentCourseEvaluationsViewRespond>(studentEvaluationResults.Error);
        
        var studentEvaluationResultsEnumerable = _studentEvaluationResultMapper.MapListToSingle(studentEvaluationResults.Value);
        
        var response = new GetStudentCourseEvaluationsViewRespond
        {
            StudentEvidences = studentsEvidences,
            StudentEvaluationResults = studentEvaluationResultsEnumerable
        };

        return response;
    }
}