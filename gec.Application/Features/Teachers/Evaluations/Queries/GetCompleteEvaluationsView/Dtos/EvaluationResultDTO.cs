namespace gec.Application.Features.Teachers.Evaluations.Queries.GetCompleteEvaluationsView.Dtos;

public class EvaluationResultDTO
{
    public string Id { get; set; } = ""; // Matches the Id of an EvaluationStructureDTO
    public string AchievementLevel { get; set; } = "";
    public string Comments { get; set; } = "";
    public bool IsEvaluated { get; set; }
}