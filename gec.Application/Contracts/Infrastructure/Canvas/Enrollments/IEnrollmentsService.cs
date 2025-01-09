using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models;

namespace gec.Application.Contracts.Infrastructure.Canvas.Enrollments;

public interface IEnrollmentsService
{
    Task<Result<List<Enrollment>>> GetStudentsByCourseAsync(string courseId);
}