using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Canvas.Api;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models;

namespace gec.Infrastructure.Canvas.Enrollments;

public class EnrollmentsService : IEnrollmentsService
{
    private readonly ICanvasApiClient _canvasApiClient;

    public EnrollmentsService(ICanvasApiClient canvasApiClient)
    {
        _canvasApiClient = canvasApiClient;
    }

    public async Task<Result<List<Enrollment>>> GetStudentsByCourseAsync(string courseId)
    {
        var endpoint = $"/api/v1/courses/{courseId}/enrollments?type[]=StudentEnrollment";
        var enrollments = await _canvasApiClient.GetAsync<List<Enrollment>>(endpoint);
        return enrollments;
    }
}