namespace gec.Application.Features.Instructors.Evaluations.Queries.GetCompleteEvaluationsView.Models;

public class CourseState
{
    public int TotalStudents { get; set; }
    public int EvaluatedStudents { get; set; }
    public int PendingStudents { get; set; }
    public string EvaluationStatus { get; set; } = ""; // Pending, Evaluated
}