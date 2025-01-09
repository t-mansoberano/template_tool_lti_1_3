namespace gec.Application.Features.Instructors.Evaluations.Queries.GetCompleteEvaluationsView.Models;

public class EvaluationStructure
{
    public string Id { get; set; } = ""; // ID of the competency or subcompetency
    public string Key { get; set; } = ""; // Key of the competency or subcompetency
    public string Name { get; set; } = ""; // Name of the competency or subcompetency
    public string Description { get; set; } = ""; // Description of the competency or subcompetency
    public string Type { get; set; } = ""; // "Competency" or "Subcompetency"
    public string? ParentId { get; set; } // ID of the parent competency, null if it's a top-level competency
    public string? ParentName { get; set; } // Name of the parent competency, null if it's a top-level competency

    public IEnumerable<Descriptor> Descriptors { get; set; } =
        new List<Descriptor>(); // Options: Highlighted, Solid, Basic, etc.
}