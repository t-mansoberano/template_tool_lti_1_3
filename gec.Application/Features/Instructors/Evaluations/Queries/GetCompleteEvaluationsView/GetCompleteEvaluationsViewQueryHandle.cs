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
    private readonly IMapper<Enrollment, Student> _canvasEnrolledStudentMapper;

    public GetCompleteEvaluationsViewHandle(ISessionStorageService sessionStorageService,
        IMapper<Enrollment, Student> canvasEnrolledStudentMapper,
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

            // Datos dummy de las evaluaciones de los estudiantes
            var random = new Random();
            var students = studentEvaluations.Select(i => new Student
            {
                Id = i.Id,
                LoginId = i.LoginId,
                Name = i.Name,
                Status = random.Next(2) == 0 ? "Pending" : "Evaluated",
                TotalEvaluations = 5,
                CompletedEvaluations = 3,
                PendingEvaluations = 2,
            });

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
                EvaluationStructures = evaluationStructures,
                Students = students,
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<GetCompleteEvaluationsViewRespond>($"Error occurred: {ex.Message}");
        }
    }
}