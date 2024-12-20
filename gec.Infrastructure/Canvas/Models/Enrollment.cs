namespace gec.Infrastructure.Canvas.Models;

public class Enrollment
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long CourseId { get; set; }
    public string Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long? AssociatedUserId { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public long CourseSectionId { get; set; }
    public long RootAccountId { get; set; }
    public bool LimitPrivilegesToCourseSection { get; set; }
    public string EnrollmentState { get; set; }
    public string Role { get; set; }
    public int RoleId { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public DateTime? LastAttendedAt { get; set; }
    public int TotalActivityTime { get; set; }
    public Grades Grades { get; set; }
    public string SisAccountId { get; set; }
    public string SisCourseId { get; set; }
    public string CourseIntegrationId { get; set; }
    public string SisSectionId { get; set; }
    public string SectionIntegrationId { get; set; }
    public string SisUserId { get; set; }
    public string HtmlUrl { get; set; }
    public User User { get; set; }
}
