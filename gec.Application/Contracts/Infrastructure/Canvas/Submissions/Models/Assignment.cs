namespace gec.Application.Contracts.Infrastructure.Canvas.Submissions.Models;

public class Assignment
{
    public long Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime? DueAt { get; set; }
    public DateTime? UnlockAt { get; set; }
    public DateTime? LockAt { get; set; }
    public double? PointsPossible { get; set; }
    public string GradingType { get; set; } = string.Empty;
    public long AssignmentGroupId { get; set; }
    public long? GradingStandardId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool PeerReviews { get; set; }
    public bool AutomaticPeerReviews { get; set; }
    public int Position { get; set; }
    public bool GradeGroupStudentsIndividually { get; set; }
    public bool AnonymousPeerReviews { get; set; }
    public long? GroupCategoryId { get; set; }
    public bool PostToSis { get; set; }
    public bool ModeratedGrading { get; set; }
    public bool OmitFromFinalGrade { get; set; }
    public bool IntraGroupPeerReviews { get; set; }
    public bool AnonymousInstructorAnnotations { get; set; }
    public bool AnonymousGrading { get; set; }
    public bool GradersAnonymousToGraders { get; set; }
    public int GraderCount { get; set; }
    public bool GraderCommentsVisibleToGraders { get; set; }
    public long? FinalGraderId { get; set; }
    public bool GraderNamesVisibleToFinalGrader { get; set; }
    public int AllowedAttempts { get; set; }
    public long? AnnotatableAttachmentId { get; set; }
    public bool HideInGradebook { get; set; }
    public string SecureParams { get; set; } = string.Empty;
    public string LtiContextId { get; set; } = string.Empty;
    public long CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> SubmissionTypes { get; set; } = new();
    public bool HasSubmittedSubmissions { get; set; }
    public bool DueDateRequired { get; set; }
    public int MaxNameLength { get; set; }
    public bool InClosedGradingPeriod { get; set; }
    public bool GradedSubmissionsExist { get; set; }
    public bool IsQuizAssignment { get; set; }
    public bool CanDuplicate { get; set; }
    public long? OriginalCourseId { get; set; }
    public long? OriginalAssignmentId { get; set; }
    public string? OriginalLtiResourceLinkId { get; set; }
    public string? OriginalAssignmentName { get; set; }
    public long? OriginalQuizId { get; set; }
    public string WorkflowState { get; set; } = string.Empty;
    public bool ImportantDates { get; set; }
    public bool Muted { get; set; }
    public string HtmlUrl { get; set; } = string.Empty;
    public bool HasOverrides { get; set; }
    public int NeedsGradingCount { get; set; }
    public string? SisAssignmentId { get; set; }
    public string? IntegrationId { get; set; }
    public Dictionary<string, object> IntegrationData { get; set; } = new();
    public long QuizId { get; set; }
    public bool AnonymousSubmissions { get; set; }
    public bool Published { get; set; }
    public bool Unpublishable { get; set; }
    public bool OnlyVisibleToOverrides { get; set; }
    public bool VisibleToEveryone { get; set; }
    public bool LockedForUser { get; set; }
    public string SubmissionsDownloadUrl { get; set; } = string.Empty;
    public bool PostManually { get; set; }
    public bool AnonymizeStudents { get; set; }
    public bool RequireLockdownBrowser { get; set; }
    public bool RestrictQuantitativeData { get; set; }
}