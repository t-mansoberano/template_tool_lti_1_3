using gec.Application.Common;
using gec.Application.Contracts.Infrastructure.Canvas.Submissions.Models;
using gec.Application.Contracts.Server.Configuration;
using gec.Application.Features.Instructors.Evaluations.Dto;

namespace gec.Application.Features.Instructors.Evaluations.Mappers;

public class CanvasSubmissionStudentMapper : IMapper<Submission, StudentEvidences>
{
    private readonly IAppSettingsService _appSettingsService;

    public CanvasSubmissionStudentMapper(IAppSettingsService appSettingsService)
    {
        _appSettingsService = appSettingsService;
    }

    // ✅ Mapea un solo Submission a StudentEvaluation
    public StudentEvidences Map(Submission input)
    {
        return new StudentEvidences
        {
            StudentId = input.UserId.ToString(),
            Evidences = new List<Evidence> { MapEvidence(input, input.UserId) },
        };
    }

    // ✅ Mapea una lista de Submission a una lista de StudentEvaluation (1:1)
    public IEnumerable<StudentEvidences> Map(IEnumerable<Submission> inputs)
    {
        return inputs.Select(Map);
    }

    // ✅ Mapea una lista de Submission a un único StudentEvaluation consolidado
    public StudentEvidences MapListToSingle(IEnumerable<Submission> inputs)
    {
        // Filtrar solo los assignments que comiencen con "Evidencia."
        var filteredSubmissions = inputs.Where(s => s.Assignment.Name.StartsWith("Evidencia.", StringComparison.OrdinalIgnoreCase)).ToList();

        if (!filteredSubmissions.Any())
            return new StudentEvidences();
        
        var firstSubmission = filteredSubmissions.First();
        var user = firstSubmission.User;

        return new StudentEvidences
        {
            StudentId = user.Id.ToString(),
            Evidences = filteredSubmissions.Select(s => MapEvidence(s, user.Id)).ToList(),
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