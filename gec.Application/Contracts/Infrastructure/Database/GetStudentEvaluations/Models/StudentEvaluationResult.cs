namespace gec.Application.Contracts.Infrastructure.Database.GetStudentEvaluations.Models;

public class StudentEvaluationResult
{
    public string Id { get; set; } = ""; // Matches the Id of an EvaluationStructureDTO
    public string AchievementLevel { get; set; } = "";
    public string Comments { get; set; } = "";
    public bool IsEvaluated { get; set; }    
}