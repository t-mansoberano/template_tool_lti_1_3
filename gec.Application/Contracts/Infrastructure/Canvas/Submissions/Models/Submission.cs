namespace gec.Application.Contracts.Infrastructure.Canvas.Submissions.Models;

public class Submission
{
    public long Id { get; set; }
    public string? Body { get; set; }
    public string? Url { get; set; }
    public string? Grade { get; set; }
    public double? Score { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public long AssignmentId { get; set; }
    public long UserId { get; set; }
    public string? SubmissionType { get; set; }
    public string WorkflowState { get; set; } = string.Empty;
    public bool GradeMatchesCurrentSubmission { get; set; }
    public DateTime? GradedAt { get; set; }
    public long? GraderId { get; set; }
    public int? Attempt { get; set; }
    public DateTime? CachedDueDate { get; set; }
    public bool? Excused { get; set; }
    public string? LatePolicyStatus { get; set; }
    public double? PointsDeducted { get; set; }
    public long? GradingPeriodId { get; set; }
    public int? ExtraAttempts { get; set; }
    public DateTime? PostedAt { get; set; }
    public bool RedoRequest { get; set; }
    public string? CustomGradeStatusId { get; set; }
    public string? Sticker { get; set; }
    public bool Late { get; set; }
    public bool Missing { get; set; }
    public int SecondsLate { get; set; }
    public string? EnteredGrade { get; set; }
    public double? EnteredScore { get; set; }
    public string PreviewUrl { get; set; } = string.Empty;
    public List<SubmissionComment> SubmissionComments { get; set; } = new();
    public Assignment Assignment { get; set; } = new();
    public User User { get; set; } = new();
    public string AnonymousId { get; set; } = string.Empty;
}
