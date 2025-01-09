namespace gec.Application.Features.Instructors.Evaluations.Queries.GetCompleteEvaluationsView.Models;

public class StudentEvaluation
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Status { get; set; } = ""; // Evaluated or Pending
    public int TotalEvaluations { get; set; } // Total evaluations (e.g., 10)
    public int CompletedEvaluations { get; set; } // Completed evaluations (e.g., 2)
    public int PendingEvaluations { get; set; } // Pending evaluations (e.g., 8)
    public IEnumerable<Evidence> Evidences { get; set; } = new List<Evidence>();
    public IEnumerable<EvaluationResult> EvaluationResults { get; set; } = new List<EvaluationResult>();
}