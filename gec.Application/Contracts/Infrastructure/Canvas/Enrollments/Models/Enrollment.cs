namespace gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models;

public class Enrollment
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long CourseId { get; set; }
    public string Type { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long? AssociatedUserId { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public long CourseSectionId { get; set; }
    public long RootAccountId { get; set; }
    public bool LimitPrivilegesToCourseSection { get; set; }
    public string EnrollmentState { get; set; } = null!;
    public string Role { get; set; } = null!;
    public int RoleId { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public DateTime? LastAttendedAt { get; set; }
    public int TotalActivityTime { get; set; }
    public Grades Grades { get; set; } = null!;
    public string SisAccountId { get; set; } = null!;
    public string SisCourseId { get; set; } = null!;
    public string CourseIntegrationId { get; set; } = null!;
    public string SisSectionId { get; set; } = null!;
    public string SectionIntegrationId { get; set; } = null!;
    public string SisUserId { get; set; } = null!;
    public string HtmlUrl { get; set; } = null!;
    public User User { get; set; } = null!;
}