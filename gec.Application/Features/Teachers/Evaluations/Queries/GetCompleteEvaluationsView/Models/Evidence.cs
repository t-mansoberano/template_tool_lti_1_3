namespace gec.Application.Features.Teachers.Evaluations.Queries.GetCompleteEvaluationsView.Models;

public class Evidence
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Feedback { get; set; } = "";
    public int Grade { get; set; }
    public string SpeedGraderLink { get; set; } = "";
    public string FileType { get; set; } = ""; // Example: "audio", "video", "other file".    
}