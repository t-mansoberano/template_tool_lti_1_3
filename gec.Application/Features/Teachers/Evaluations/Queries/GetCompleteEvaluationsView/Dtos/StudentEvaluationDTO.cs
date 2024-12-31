namespace gec.Application.Features.Teachers.Evaluations.Queries.GetCompleteEvaluationsView.Dtos;

public class StudentEvaluationDTO
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Status { get; set; } = ""; // Evaluated or Pending
    public int TotalEvaluations { get; set; } // Total evaluations (e.g., 10)
    public int CompletedEvaluations { get; set; } // Completed evaluations (e.g., 2)
    public int PendingEvaluations { get; set; } // Pending evaluations (e.g., 8)
    public IEnumerable<EvidenceDTO> Evidences { get; set; } = new List<EvidenceDTO>();
    public IEnumerable<EvaluationResultDTO> EvaluationResults { get; set; } = new List<EvaluationResultDTO>();
}