namespace gec.Application.Features.Instructors.Evaluations.Dto;

public class StudentEvaluationResults
{
    public string StudentId { get; set; } = string.Empty;
    public IEnumerable<EvaluationResult> EvaluationResults { get; set; } = new List<EvaluationResult>();
}