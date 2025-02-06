using CSharpFunctionalExtensions;
using gec.Application.Common;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models;
using gec.Application.Contracts.Infrastructure.Lti.Models;
using gec.Application.Contracts.Server.Session;
using gec.Application.Features.Instructors.Evaluations.Dto;
using MediatR;
using Course = gec.Application.Features.Instructors.Evaluations.Dto.Course;

namespace gec.Application.Features.Instructors.Evaluations.Queries.GetCompleteEvaluationsView;

public class GetCompleteEvaluationsViewHandle : IRequestHandler<GetCompleteEvaluationsViewQuery,
    Result<GetCompleteEvaluationsViewRespond>>
{
    private readonly ISessionStorageService _sessionStorageService;
    private readonly IEnrollmentsService _enrollmentsService;
    private readonly IMapper<Enrollment, StudentEvaluation> _canvasEnrolledStudentMapper;

    public GetCompleteEvaluationsViewHandle(ISessionStorageService sessionStorageService,
        IMapper<Enrollment, StudentEvaluation> canvasEnrolledStudentMapper,
        IEnrollmentsService enrollmentsService)
    {
        _sessionStorageService = sessionStorageService;
        _enrollmentsService = enrollmentsService;
        _canvasEnrolledStudentMapper = canvasEnrolledStudentMapper;
    }

    public async Task<Result<GetCompleteEvaluationsViewRespond>> Handle(GetCompleteEvaluationsViewQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult =
                await new GetCompleteEvaluationsViewQueryValidator().ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Failure<GetCompleteEvaluationsViewRespond>(validationResult.ErrorMessages());

            var ltiContex = _sessionStorageService.Retrieve<LtiContext>("LtiContext");
            if (ltiContex.IsFailure)
                return Result.Failure<GetCompleteEvaluationsViewRespond>(ltiContex.Error);

            var studentsFromApi = await _enrollmentsService.GetStudentsByCourseAsync(request.CourseId);
            if (studentsFromApi.IsFailure)
                return Result.Failure<GetCompleteEvaluationsViewRespond>(studentsFromApi.Error);

            var studentEvaluations = _canvasEnrolledStudentMapper.Map(studentsFromApi.Value);

            var course = new Course
            {
                Id = ltiContex.Value.Course.Id,
                Key = ltiContex.Value.Course.Label,
                Name = ltiContex.Value.Course.Title,
            };

            // Datos dummy del estado del curso
            var courseState = new CourseState
            {
                TotalStudents = 10,
                EvaluatedStudents = 6,
                PendingStudents = 4,
                EvaluationStatus = "Faltan estudiantes por evaluar"
            };

            // Datos dummy de estudiantes
            var students = studentEvaluations.Select(i => new StudentEvaluation
            {
                Id = i.Id,
                LoginId = i.LoginId,
                Name = i.Name,
                Status = "Pending",
                TotalEvaluations = 5,
                CompletedEvaluations = 3,
                PendingEvaluations = 2,
                Evidences = new List<Evidence>
                {
                    new()
                    {
                        Id = $"EVIDENCE{i}A",
                        Name = $"Evidence{i} A",
                        Feedback = $"Good job {i}!",
                        Grade = 85,
                        SpeedGraderLink = "https://speedgrader.example.com",
                        FileType = "audio"
                    },
                    new()
                    {
                        Id = $"EVIDENCE{i}B",
                        Name = $"Evidence{i} B",
                        Feedback = $"Needs improvement {i}.",
                        Grade = 70,
                        SpeedGraderLink = "https://speedgrader.example.com",
                        FileType = "video"
                    },
                    new()
                    {
                        Id = $"EVIDENCE{i}C",
                        Name = $"Evidence{i} C",
                        Feedback = $"Needs improvement {i}.",
                        Grade = 60,
                        SpeedGraderLink = "https://speedgrader.example.com",
                        FileType = "other"
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
                        IsEvaluated = false
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
                        new()
                        {
                            Id = "LEVEL1", Level = "Destacado",
                            Description =
                                "Posee una comprensión profunda sobre técnicas de investigación cuantitativa y cualitativa, que le permite recopilar y analizar sistemáticamente información. Establece las dimensiones y variables sustanciales del problema, que considera para diseñar instrumentos confiables y válidos. Realiza consistentemente análisis sistémicos y multidisciplinarios durante la investigación."
                        },
                        new()
                        {
                            Id = "LEVEL2", Level = "Sólido",
                            Description =
                                "Posee una comprensión profunda sobre técnicas de investigación cuantitativa y cualitativa, que le permite recopilar y analizar sistemáticamente información. Establece las dimensiones y variables sustanciales del problema, que considera para diseñar instrumentos confiables y válidos. Realiza consistentemente análisis sistémicos y multidisciplinarios durante la investigación."
                        },
                        new()
                        {
                            Id = "LEVEL3", Level = "Básico",
                            Description =
                                "Posee una comprensión profunda sobre técnicas de investigación cuantitativa y cualitativa, que le permite recopilar y analizar sistemáticamente información. Establece las dimensiones y variables sustanciales del problema, que considera para diseñar instrumentos confiables y válidos. Realiza consistentemente análisis sistémicos y multidisciplinarios durante la investigación."
                        },
                        new()
                        {
                            Id = "LEVEL4", Level = "Incipiente",
                            Description =
                                "Posee una comprensión profunda sobre técnicas de investigación cuantitativa y cualitativa, que le permite recopilar y analizar sistemáticamente información. Establece las dimensiones y variables sustanciales del problema, que considera para diseñar instrumentos confiables y válidos. Realiza consistentemente análisis sistémicos y multidisciplinarios durante la investigación."
                        }
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
                        new()
                        {
                            Id = "LEVEL1", Level = "Destacado",
                            Description =
                                "Posee una comprensión profunda sobre técnicas de investigación cuantitativa y cualitativa, que le permite recopilar y analizar sistemáticamente información. Establece las dimensiones y variables sustanciales del problema, que considera para diseñar instrumentos confiables y válidos. Realiza consistentemente análisis sistémicos y multidisciplinarios durante la investigación."
                        },
                        new()
                        {
                            Id = "LEVEL2", Level = "Sólido",
                            Description =
                                "Posee una comprensión profunda sobre técnicas de investigación cuantitativa y cualitativa, que le permite recopilar y analizar sistemáticamente información. Establece las dimensiones y variables sustanciales del problema, que considera para diseñar instrumentos confiables y válidos. Realiza consistentemente análisis sistémicos y multidisciplinarios durante la investigación."
                        },
                        new()
                        {
                            Id = "LEVEL3", Level = "Básico",
                            Description =
                                "Posee una comprensión profunda sobre técnicas de investigación cuantitativa y cualitativa, que le permite recopilar y analizar sistemáticamente información. Establece las dimensiones y variables sustanciales del problema, que considera para diseñar instrumentos confiables y válidos. Realiza consistentemente análisis sistémicos y multidisciplinarios durante la investigación."
                        },
                        new()
                        {
                            Id = "LEVEL4", Level = "Incipiente",
                            Description =
                                "Posee una comprensión profunda sobre técnicas de investigación cuantitativa y cualitativa, que le permite recopilar y analizar sistemáticamente información. Establece las dimensiones y variables sustanciales del problema, que considera para diseñar instrumentos confiables y válidos. Realiza consistentemente análisis sistémicos y multidisciplinarios durante la investigación."
                        }
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