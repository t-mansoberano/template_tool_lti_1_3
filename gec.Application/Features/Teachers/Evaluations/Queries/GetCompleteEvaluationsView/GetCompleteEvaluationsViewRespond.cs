using gec.Application.Features.Teachers.Evaluations.Queries.GetCompleteEvaluationsView.Dtos;

namespace gec.Application.Features.Teachers.Evaluations.Queries.GetCompleteEvaluationsView;

public class GetCompleteEvaluationsViewRespond
{
    public CourseDTO Course { get; set; }
    public CourseStateDTO CourseState { get; set; }
    public IEnumerable<StudentEvaluationDTO> Students { get; set; }
    public IEnumerable<EvaluationStructureDTO> EvaluationStructures { get; set; }
}