namespace gec.Application.Features.Instructors.Evaluations.Dto;

public class StudentEvidences
{
    public string StudentId { get; set; } = string.Empty;
    public IEnumerable<Evidence> Evidences { get; set; } = new List<Evidence>();
}