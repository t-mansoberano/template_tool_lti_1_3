namespace gec.Application.Contracts.Infrastructure.Canvas.Enrollments;

public class Grades
{
    public string HtmlUrl { get; set; } = null!;
    public double? CurrentGrade { get; set; }
    public double? CurrentScore { get; set; }
    public double? FinalGrade { get; set; }
    public double? FinalScore { get; set; }
    public double? UnpostedCurrentScore { get; set; }
    public double? UnpostedCurrentGrade { get; set; }
    public double? UnpostedFinalScore { get; set; }
    public double? UnpostedFinalGrade { get; set; }
}