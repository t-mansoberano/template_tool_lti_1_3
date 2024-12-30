using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Canvas;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments;
using gec.Application.Contracts.Infrastructure.Lti.Models;
using gec.Application.Contracts.Server;

namespace gec.Infrastructure.Canvas.Enrollments;

public class EnrollmentsService : IEnrollmentsService
{
    private readonly ICanvasApiClient _canvasApiClient;
    private readonly ISessionStorageService _sessionStorageService;

    public EnrollmentsService(ICanvasApiClient canvasApiClient, ISessionStorageService sessionStorageService)
    {
        _canvasApiClient = canvasApiClient;
        _sessionStorageService = sessionStorageService;
    }
    
    public async Task<Result<List<Enrollment>>> GetStudentsByCourseAsync()
    {
        var ltiContext = _sessionStorageService.Retrieve<LtiContext>("LtiContext");
        if (ltiContext.IsFailure) 
            return Result.Failure<List<Enrollment>>(ltiContext.Error);

        var endpoint = $"/api/v1/courses/{ltiContext.Value.Course.Id}/enrollments?type[]=StudentEnrollment";
        var enrollments = await _canvasApiClient.GetAsync<List<Enrollment>>(endpoint);
        return enrollments;
    }
}