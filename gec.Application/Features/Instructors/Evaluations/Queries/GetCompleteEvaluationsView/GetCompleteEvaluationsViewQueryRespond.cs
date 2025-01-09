using gec.Application.Features.Instructors.Evaluations.Queries.GetCompleteEvaluationsView.Models;

namespace gec.Application.Features.Instructors.Evaluations.Queries.GetCompleteEvaluationsView;

public class GetCompleteEvaluationsViewRespond
{
    public Course Course { get; set; } = new();
    public CourseState CourseState { get; set; } = new();
    public IEnumerable<StudentEvaluation> Students { get; set; } = new List<StudentEvaluation>();
    public IEnumerable<EvaluationStructure> EvaluationStructures { get; set; } = new List<EvaluationStructure>();
}