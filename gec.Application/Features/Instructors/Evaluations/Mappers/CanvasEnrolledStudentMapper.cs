using gec.Application.Common;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models;
using gec.Application.Features.Instructors.Evaluations.Dto;

namespace gec.Application.Features.Instructors.Evaluations.Mappers;

public class CanvasEnrolledStudentMapper : IMapper<Enrollment, StudentEvaluation>
{
    public StudentEvaluation Map(Enrollment input)
    {
        return new StudentEvaluation()
        {
            Id = input.UserId.ToString(),
            Name = input.User.Name,
        };
    }

    public IEnumerable<StudentEvaluation> Map(IEnumerable<Enrollment> inputs)
    {
        return inputs.Select(Map);
    }
}