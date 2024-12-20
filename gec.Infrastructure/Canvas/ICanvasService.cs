using gec.Infrastructure.Canvas.Models;

namespace gec.Infrastructure.Canvas;

public interface ICanvasService
{
    Task<IEnumerable<Student>> GetStudentsByCourseAsync(string accessToken, string courseId);
}