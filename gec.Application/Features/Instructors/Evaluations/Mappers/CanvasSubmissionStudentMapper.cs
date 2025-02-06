using gec.Application.Common;
using gec.Application.Contracts.Infrastructure.Canvas.Submissions.Models;
using gec.Application.Contracts.Server.Configuration;
using gec.Application.Features.Instructors.Evaluations.Dto;

namespace gec.Application.Features.Instructors.Evaluations.Mappers;

public class CanvasSubmissionStudentMapper : IMapper<Submission, StudentEvaluation>
{
    private readonly IAppSettingsService _appSettingsService;

    public CanvasSubmissionStudentMapper(IAppSettingsService appSettingsService)
    {
        _appSettingsService = appSettingsService;
    }

    // ✅ Mapea un solo Submission a StudentEvaluation
    public StudentEvaluation Map(Submission input)
    {
        return new StudentEvaluation
        {
            Id = input.UserId.ToString(),
            LoginId = input.User.LoginId,
            Name = input.User.Name,
            Status = string.IsNullOrEmpty(input.Grade) && string.IsNullOrEmpty(input.EnteredGrade)
                ? "Pending"
                : "Evaluated",
            TotalEvaluations = 1,
            CompletedEvaluations =
                string.IsNullOrEmpty(input.Grade) && string.IsNullOrEmpty(input.EnteredGrade) ? 0 : 1,
            PendingEvaluations = string.IsNullOrEmpty(input.Grade) && string.IsNullOrEmpty(input.EnteredGrade) ? 1 : 0,
            Evidences = new List<Evidence> { MapEvidence(input, input.UserId) },
            EvaluationResults = new List<EvaluationResult> { MapEvaluationResult(input) }
        };
    }

    // ✅ Mapea una lista de Submission a una lista de StudentEvaluation (1:1)
    public IEnumerable<StudentEvaluation> Map(IEnumerable<Submission> inputs)
    {
        return inputs.Select(Map);
    }

    // ✅ Mapea una lista de Submission a un único StudentEvaluation consolidado
    public StudentEvaluation MapListToSingle(IEnumerable<Submission> inputs)
    {
        if (inputs == null || !inputs.Any())
            return new StudentEvaluation();

        // Filtrar solo los assignments que comiencen con "Evidencia."
        var filteredSubmissions = inputs.Where(s => s.Assignment.Name.StartsWith("Evidencia.", StringComparison.OrdinalIgnoreCase)).ToList();

        var firstSubmission = filteredSubmissions.First();
        var user = firstSubmission.User;

        return new StudentEvaluation
        {
            Id = user.Id.ToString(),
            LoginId = user.LoginId,
            Name = user.Name,
            Status = "Pending",
            TotalEvaluations = 5,
            CompletedEvaluations = 3,
            PendingEvaluations = 2,
            Evidences = filteredSubmissions.Select(s => MapEvidence(s, user.Id)).ToList(),
            EvaluationResults = new List<EvaluationResult>
            {
                new()
                {
                    Id = $"COMP_A",
                    AchievementLevel = "Solid",
                    Comments = "Solid understanding.",
                    IsEvaluated = true
                },
                new()
                {
                    Id = $"COMP_B",
                    AchievementLevel = "Basic",
                    Comments = "Needs improvement.",
                    IsEvaluated = false
                }
            }
        };
    }

    // ✅ Mapea un Submission en Evidence
    private Evidence MapEvidence(Submission submission, long studentId)
    {
        var instructorComments = submission.SubmissionComments
            .Where(comment => comment.AuthorId != studentId)
            .Select(comment => comment.Comment)
            .Where(comment => !string.IsNullOrWhiteSpace(comment))
            .ToList();
        
        return new Evidence
        {
            Id = submission.Id.ToString(),
            Name = submission.Assignment.Name,
            Feedback = instructorComments.Any() ? string.Join(" | ", instructorComments) : "No hay comentarios del instructor",
            // Feedback = instructorComments ?? "No hay comentarios del instructor",
            Grade = (int)(submission.Score ?? 0),
            SpeedGraderLink = $"{_appSettingsService.Canvas.ApiBaseUrl}/courses/{submission.Assignment.CourseId}/gradebook/speed_grader?assignment_id={submission.AssignmentId}",
            FileType = GetFileType(submission.SubmissionType),
            PreviewUrl = submission.PreviewUrl
        };
    }

    // ✅ Mapea un Submission en EvaluationResult
    private EvaluationResult MapEvaluationResult(Submission submission)
    {
        return new EvaluationResult
        {
            Id = submission.AssignmentId.ToString(),
            AchievementLevel = submission.Grade ?? submission.EnteredGrade ?? "N/A",
            Comments = submission.SubmissionComments.Any() ? submission.SubmissionComments.First().Comment : "",
            IsEvaluated = !string.IsNullOrEmpty(submission.Grade) || !string.IsNullOrEmpty(submission.EnteredGrade)
        };
    }

    // ✅ Determina el tipo de archivo basado en SubmissionType
    private string GetFileType(string? submissionType)
    {
        return submissionType switch
        {
            "media_recording" => "video",
            "audio_recording" => "audio",
            "online_upload" => "other",
            _ => ""
        };
    }
}