namespace gec.Application.Features.Teachers.Evaluations.Queries.GetTestForCanvasAPI;

public class GetTestForCanvasAPIRespond
{
    public List<Enrollment> Enrollments { get; set; }
}

public class Enrollment
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long CourseId { get; set; }
    public string Type { get; set; } = null!;
    public Grades Grades { get; set; } = null!;
    public User User { get; set; } = null!;
}

public class Grades
{
    public string HtmlUrl { get; set; } = null!;
    public double? CurrentGrade { get; set; }
    public double? CurrentScore { get; set; }
    public double? FinalGrade { get; set; }
    public double? FinalScore { get; set; }
}

public class User
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
}

