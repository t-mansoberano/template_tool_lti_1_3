namespace gec.Application.Features.Teachers.Evaluations.Queries.GetCompleteEvaluationsView.Dtos;

public class CourseStateDTO
{
    public int TotalStudents { get; set; }
    public int EvaluatedStudents { get; set; }
    public int PendingStudents { get; set; }
    public string EvaluationStatus { get; set; } = ""; // Pending, Evaluated
}