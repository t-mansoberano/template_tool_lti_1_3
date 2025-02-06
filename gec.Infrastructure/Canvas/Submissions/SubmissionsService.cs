using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Canvas.Api;
using gec.Application.Contracts.Infrastructure.Canvas.Submissions;
using gec.Application.Contracts.Infrastructure.Canvas.Submissions.Models;

namespace gec.Infrastructure.Canvas.Submissions;

public class SubmissionsService : ISubmissionsService
{
    private readonly ICanvasApiClient _canvasApiClient;

    public SubmissionsService(ICanvasApiClient canvasApiClient)
    {
        _canvasApiClient = canvasApiClient;
    }

    public async Task<Result<List<Submission>>> GetSubmissionsByStudentAsync(string courseId, string studentId)
    {
        string endpoint =
            $"/api/v1/courses/{courseId}/students/submissions?student_ids[]={studentId}&include[]=assignment&include[]=user&include[]=submission_comments";
        return await _canvasApiClient.GetPaginatedAsync<Submission>(endpoint);
    }
}