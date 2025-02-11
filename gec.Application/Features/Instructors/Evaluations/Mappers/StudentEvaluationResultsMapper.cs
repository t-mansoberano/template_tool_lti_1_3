using gec.Application.Common;
using gec.Application.Contracts.Infrastructure.Database.GetStudentEvaluations.Models;
using gec.Application.Features.Instructors.Evaluations.Dto;

namespace gec.Application.Features.Instructors.Evaluations.Mappers;

public class StudentEvaluationResultsMapper : IMapper<StudentEvaluationResult, StudentEvaluationResults>
{
    public StudentEvaluationResults Map(StudentEvaluationResult input)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<StudentEvaluationResults> Map(IEnumerable<StudentEvaluationResult> inputs)
    {
        throw new NotImplementedException();
    }

    public StudentEvaluationResults MapListToSingle(IEnumerable<StudentEvaluationResult> inputs)
    {
        // Suponiendo que todos los resultados pertenezcan a un único estudiante, 
        // se puede extraer el StudentId del primer registro o definirlo de otra manera
        var studentId = inputs.First().Id; 

        // Se construye la lista de EvaluationResult mapeando cada StudentEvaluationResult
        var evaluationResults = inputs.Select(result => new EvaluationResult
        {
            Id = result.Id,
            AchievementLevel = result.AchievementLevel,
            Comments = result.Comments,
            IsEvaluated = result.IsEvaluated
        });

        return new StudentEvaluationResults
        {
            StudentId = studentId,
            EvaluationResults = evaluationResults
        };
    }

    public StudentEvaluationResults MapWithDependencies(IEnumerable<StudentEvaluationResult> inputs, object dependencies)
    {
        throw new NotImplementedException();
    }
}
