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

            // Datos dummy de las evaluaciones de los estudiantes
            var random = new Random();
            var students = studentEvaluations.Select(i =>
            {
                var status = random.Next(2) == 0 ? "Pending" : "Completed";
                return new Student
                {
                    Id = i.Id,
                    LoginId = i.LoginId,
                    Name = i.Name,
                    Status = status,
                    TotalEvaluations = 5,
                    CompletedEvaluations = status == "Completed" ? 5 : 3,
                    PendingEvaluations = status == "Completed" ? 0 : 2
                };
            });

            // Datos dummy del estado del curso
            var studentsList = students.ToList();
            
            var totalStudents = studentsList.Count;
            var evaluatedStudents = studentsList.Count(i => i.Status == "Completed");
            var pendingStudents = studentsList.Count(i => i.Status == "Pending");
            
            var courseState = new CourseState
            {
                TotalStudents = totalStudents,
                EvaluatedStudents = evaluatedStudents,
                PendingStudents = pendingStudents,
                EvaluationStatus = totalStudents == evaluatedStudents
                    ? "Todos los estudiantes evaluados"
                    : "Faltan estudiantes por evaluar"
            };

            // Datos dummy de la estructura de evaluación
            var structures = new List<EvaluationStructure>();
            // Lista de niveles que queremos asignar a los descriptores
            var levels = new List<string> { "Destacado", "Sólido", "Básico", "Incipiente" };

            for (int i = 0; i < 20; i++)
            {
                // Para cada EvaluationStructure, creamos la lista de 4 Descriptor, uno por cada nivel.
                var descriptors = new List<Descriptor>();
                foreach (var level in levels)
                {
                    descriptors.Add(new Descriptor
                    {
                        Id = Guid.NewGuid().ToString(),
                        Level = level,
                        Description = $"Descripción para el nivel {level}"
                    });
                }

                // Creamos la EvaluationStructure asignando la lista completa de descriptores
                var evaluationStructure = new EvaluationStructure
                {
                    Id = (i + 1).ToString(),
                    Key = $"Key-{i + 1}",
                    Name = $"Competencia {i + 1}",
                    Description = $"Descripción de la competencia {i + 1}",
                    Type = "Competency", // Ajusta el valor según corresponda
                    ParentId = null,     // O asigna un valor si aplica
                    ParentName = null,   // O asigna un valor si aplica
                    Descriptors = descriptors
                };

                structures.Add(evaluationStructure);
            }

            var evaluationStructures = structures;
            
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