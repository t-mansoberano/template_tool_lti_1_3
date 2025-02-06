using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Canvas.Submissions.Models;

namespace gec.Application.Contracts.Infrastructure.Canvas.Submissions;

public interface ISubmissionsService
{
    Task<Result<List<Submission>>> GetSubmissionsByStudentAsync(string courseId, string studentId);
}