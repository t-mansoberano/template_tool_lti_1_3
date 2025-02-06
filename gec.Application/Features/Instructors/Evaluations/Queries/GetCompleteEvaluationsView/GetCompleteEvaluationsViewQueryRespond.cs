using gec.Application.Features.Instructors.Evaluations.Dto;

namespace gec.Application.Features.Instructors.Evaluations.Queries.GetCompleteEvaluationsView;

public class GetCompleteEvaluationsViewRespond
{
    public Course Course { get; set; } = new();
    public CourseState CourseState { get; set; } = new();
    public IEnumerable<Student> Students { get; set; } = new List<Student>();
    public Student? SelectedStudent { get; set; } = null;
    public StudentEvidences? StudentEvidences { get; set; } = null;
    public StudentEvaluationResults? StudentEvaluationResults { get; set; } = null;
    public IEnumerable<EvaluationStructure> EvaluationStructures { get; set; } = new List<EvaluationStructure>();
}