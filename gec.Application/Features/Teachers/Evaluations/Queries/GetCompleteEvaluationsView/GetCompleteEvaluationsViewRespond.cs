using gec.Application.Features.Teachers.Evaluations.Queries.GetCompleteEvaluationsView.Dtos;

namespace gec.Application.Features.Teachers.Evaluations.Queries.GetCompleteEvaluationsView;

public class GetCompleteEvaluationsViewRespond
{
    public CourseDTO Course { get; set; } = new();
    public CourseStateDTO CourseState { get; set; } = new();
    public IEnumerable<StudentEvaluationDTO> Students { get; set; } = new List<StudentEvaluationDTO>();
    public IEnumerable<EvaluationStructureDTO> EvaluationStructures { get; set; } = new List<EvaluationStructureDTO>();
}