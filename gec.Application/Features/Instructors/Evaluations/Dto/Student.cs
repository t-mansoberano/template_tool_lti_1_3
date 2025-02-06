namespace gec.Application.Features.Instructors.Evaluations.Dto;

public class Student
{
    public long Id { get; set; }
    public string LoginId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Evaluated or Pending
    public int TotalEvaluations { get; set; } // Total evaluations (e.g., 10)
    public int CompletedEvaluations { get; set; } // Completed evaluations (e.g., 2)
    public int PendingEvaluations { get; set; } // Pending evaluations (e.g., 8)
}