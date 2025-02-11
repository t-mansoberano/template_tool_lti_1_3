using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Database.GetStudentEvaluations.Models;

namespace gec.Application.Contracts.Infrastructure.Database.GetStudentEvaluations;

public interface IGetStudentEvaluationsRepository
{
    Task<Result<List<StudentEvaluationResult>>> Get(string courseId, string studentId);
}