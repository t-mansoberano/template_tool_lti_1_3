using CSharpFunctionalExtensions;
using gec.Application.Common;
using gec.Application.Features.Teachers.Evaluations.Queries.GetCompleteEvaluationsView.Models;
using MediatR;

namespace gec.Application.Features.Teachers.Evaluations.Queries.GetCompleteEvaluationsView;

public class GetCompleteEvaluationsViewHandle : IRequestHandler<GetCompleteEvaluationsViewQuery,
    Result<GetCompleteEvaluationsViewRespond>>
{
    public async Task<Result<GetCompleteEvaluationsViewRespond>> Handle(GetCompleteEvaluationsViewQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult =
                await new GetCompleteEvaluationsViewQueryValidator().ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Failure<GetCompleteEvaluationsViewRespond>(validationResult.ErrorMessages());

            await Task.Delay(100, cancellationToken); // Simula una llamada asíncrona.

            // Datos dummy del curso
            var course = new Course
            {
                Id = "COURSE001",
                Key = "COURSE_KEY",
                Name = "Sample Course"
            };

            // Datos dummy del estado del curso
            var courseState = new CourseState
            {
                TotalStudents = 10,
                EvaluatedStudents = 6,
                PendingStudents = 4,
                EvaluationStatus = "In Progress"
            };

            // Datos dummy de estudiantes
            var students = Enumerable.Range(1, 10).Select(i => new StudentEvaluation
            {
                Id = $"STUDENT{i}",
                Name = $"Student {i}",
                Status = i <= 6 ? "Evaluated" : "Pending",
                TotalEvaluations = 5,
                CompletedEvaluations = i <= 6 ? 5 : 3,
                PendingEvaluations = i <= 6 ? 0 : 2,
                Evidences = new List<Evidence>
                {
                    new()
                    {
                        Id = $"EVIDENCE{i}A",
                        Name = "Evidence A",
                        Feedback = "Good job!",
                        Grade = 85,
                        SpeedGraderLink = "https://speedgrader.example.com",
                        FileType = "pdf"
                    },
                    new()
                    {
                        Id = $"EVIDENCE{i}B",
                        Name = "Evidence B",
                        Feedback = "Needs improvement.",
                        Grade = 70,
                        SpeedGraderLink = "https://speedgrader.example.com",
                        FileType = "docx"
                    }
                },
                EvaluationResults = new List<EvaluationResult>
                {
                    new()
                    {
                        Id = $"COMP{i}A",
                        AchievementLevel = "Solid",
                        Comments = "Solid understanding.",
                        IsEvaluated = true
                    },
                    new()
                    {
                        Id = $"COMP{i}B",
                        AchievementLevel = "Basic",
                        Comments = "Needs improvement.",
                        IsEvaluated = i <= 6
                    }
                }
            }).ToList();

            // Datos dummy de la estructura de evaluación
            var evaluationStructures = new List<EvaluationStructure>
            {
                new()
                {
                    Id = "COMP1",
                    Key = "COMP_KEY_1",
                    Name = "Competency 1",
                    Description = "Description of Competency 1",
                    Type = "Competency",
                    ParentId = null,
                    ParentName = null,
                    Descriptors = new List<Descriptor>
                    {
                        new() { Id = "LEVEL1", Level = "Highlighted", Description = "Excellent understanding." },
                        new() { Id = "LEVEL2", Level = "Solid", Description = "Solid understanding." },
                        new() { Id = "LEVEL3", Level = "Basic", Description = "Basic understanding." },
                        new() { Id = "LEVEL4", Level = "Incipient", Description = "Needs improvement." }
                    }
                },
                new()
                {
                    Id = "COMP2",
                    Key = "COMP_KEY_2",
                    Name = "Competency 2",
                    Description = "Description of Competency 2",
                    Type = "Subcompetency",
                    ParentId = "COMP1",
                    ParentName = "Competency 1",
                    Descriptors = new List<Descriptor>
                    {
                        new() { Id = "LEVEL1", Level = "Highlighted", Description = "Excellent understanding." },
                        new() { Id = "LEVEL2", Level = "Solid", Description = "Solid understanding." },
                        new() { Id = "LEVEL3", Level = "Basic", Description = "Basic understanding." },
                        new() { Id = "LEVEL4", Level = "Incipient", Description = "Needs improvement." }
                    }
                }
            };

            // Construir la respuesta
            var response = new GetCompleteEvaluationsViewRespond
            {
                Course = course,
                CourseState = courseState,
                Students = students,
                EvaluationStructures = evaluationStructures
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<GetCompleteEvaluationsViewRespond>($"Error occurred: {ex.Message}");
        }
    }
}