using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Database.GetStudentEvaluations;
using gec.Application.Contracts.Infrastructure.Database.GetStudentEvaluations.Models;

namespace gec.Infrastructure.Database.GetStudentEvaluations;

public class GetStudentEvaluationsRepository : IGetStudentEvaluationsRepository
{
    public Task<Result<List<StudentEvaluationResult>>> Get(string courseId, string studentId)
    {
        var evaluations = new List<StudentEvaluationResult>();
        var random = new Random();
        // Array of possible achievement levels.
        string[] achievementLevels = { "Destacado", "Sólido", "Básico", "Incipiente", "NoElementosSuficientes" };

        for (int i = 0; i < 20; i++)
        {
            var isEvaluated = random.Next(0, 2) == 1;
            var achievementLevel = isEvaluated? achievementLevels[random.Next(achievementLevels.Length)]:"";
            var evaluation = new StudentEvaluationResult
            {
                // Generating a random GUID as Id to simulate the EvaluationStructureDTO Id.
                Id = (i + 1).ToString(),
                // Randomly selecting an achievement level.
                AchievementLevel = achievementLevel,
                // Creating a random comment.
                Comments = $"Dummy comment {random.Next(1, 1000)}",
                // Randomly choosing true or false.
                IsEvaluated =isEvaluated
            };

            evaluations.Add(evaluation);
        }

        return Task.FromResult(Result.Success(evaluations));
    }

}