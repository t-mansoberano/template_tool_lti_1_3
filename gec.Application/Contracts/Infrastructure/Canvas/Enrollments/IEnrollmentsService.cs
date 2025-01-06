using CSharpFunctionalExtensions;

namespace gec.Application.Contracts.Infrastructure.Canvas.Enrollments;

public interface IEnrollmentsService
{
    Task<Result<List<Enrollment>>> GetStudentsByCourseAsync(string courseId);
}