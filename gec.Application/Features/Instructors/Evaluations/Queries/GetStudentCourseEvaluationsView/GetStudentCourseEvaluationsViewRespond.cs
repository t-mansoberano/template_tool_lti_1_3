using gec.Application.Features.Instructors.Evaluations.Dto;

namespace gec.Application.Features.Instructors.Evaluations.Queries.GetStudentCourseEvaluationsView;

public class GetStudentCourseEvaluationsViewRespond
{
    public StudentEvaluation SelectedStudent { get; set; } = new ();
}