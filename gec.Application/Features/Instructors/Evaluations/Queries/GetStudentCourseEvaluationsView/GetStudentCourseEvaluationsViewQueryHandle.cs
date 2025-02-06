using CSharpFunctionalExtensions;
using gec.Application.Common;
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

    public GetStudentCourseEvaluationsViewQueryHandle(ISessionStorageService sessionStorageService,
        IMapper<Submission, StudentEvidences> canvasSubmissionStudentEvidenceMapper,
        ISubmissionsService submissionsService)
    {
        _canvasSubmissionStudentEvidenceMapper = canvasSubmissionStudentEvidenceMapper;
        _submissionsService = submissionsService;
    }

    public async Task<Result<GetStudentCourseEvaluationsViewRespond>> Handle(
        GetStudentCourseEvaluationsViewQuery request,
        CancellationToken cancellationToken)
    {
        var evaluationsFromApi =
            await _submissionsService.GetSubmissionsByStudentAsync(request.CourseId, request.UserId);
        if (evaluationsFromApi.IsFailure)
            return Result.Failure<GetStudentCourseEvaluationsViewRespond>(evaluationsFromApi.Error);

        var studentsEvidences = _canvasSubmissionStudentEvidenceMapper.MapListToSingle(evaluationsFromApi.Value);

        var response = new GetStudentCourseEvaluationsViewRespond
        {
            StudentEvidences = studentsEvidences
        };

        return response;
    }
}