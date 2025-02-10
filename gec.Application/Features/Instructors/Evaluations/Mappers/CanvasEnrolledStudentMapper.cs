using gec.Application.Common;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models;
using gec.Application.Features.Instructors.Evaluations.Dto;

namespace gec.Application.Features.Instructors.Evaluations.Mappers;

public class CanvasEnrolledStudentMapper : IMapper<Enrollment, Student>
{
    public Student Map(Enrollment input)
    {
        return new Student()
        {
            Id = input.UserId,
            LoginId = input.User.LoginId,
            Name = input.User.Name,
        };
    }

    public IEnumerable<Student> Map(IEnumerable<Enrollment> inputs)
    {
        return inputs.Select(Map);
    }

    public Student MapListToSingle(IEnumerable<Enrollment> inputs)
    {
        return Map(inputs.First());
    }

    public Student MapWithDependencies(IEnumerable<Enrollment> inputs, object dependencies)
    {
        throw new NotImplementedException();
    }
}