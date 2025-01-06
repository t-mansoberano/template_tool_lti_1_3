namespace gec.Application.Features.Teachers.Evaluations.Queries.GetCompleteEvaluationsView.Models;

public class Descriptor
{
    public string Id { get; set; } = "";
    public string Level { get; set; } = ""; // Example: Highlighted, Solid, Basic, Incipient
    public string Description { get; set; } = ""; // Detailed description of the level
}
