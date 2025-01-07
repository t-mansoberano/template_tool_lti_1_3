namespace gec.Application.Features.Teachers.Evaluations.Queries.GetTestForCanvasApi;

public class GetTestForCanvasApiRespond
{
    public List<Enrollment> Enrollments { get; set; } = new();
}

public class Enrollment
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long CourseId { get; set; }
    public string Type { get; set; } = "";
    public Grades Grades { get; set; } = new();
    public User User { get; set; } = new();
}

public class Grades
{
    public string HtmlUrl { get; set; } = "";
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