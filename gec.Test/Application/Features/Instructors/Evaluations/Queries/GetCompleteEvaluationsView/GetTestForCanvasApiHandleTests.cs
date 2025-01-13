using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments;
using gec.Application.Features.Instructors.Evaluations.Queries.GetTestForCanvasApi;
using Moq;
using Enrollment = gec.Application.Features.Instructors.Evaluations.Queries.GetTestForCanvasApi.Enrollment;
using Grades = gec.Application.Features.Instructors.Evaluations.Queries.GetTestForCanvasApi.Grades;
using User = gec.Application.Features.Instructors.Evaluations.Queries.GetTestForCanvasApi.User;

namespace gec.Test.Application.Features.Instructors.Evaluations.Queries.GetCompleteEvaluationsView;

[TestFixture]
public class GetTestForCanvasApiHandleTests
{
    private Mock<IEnrollmentsService> _mockEnrollmentsService;
    private GetTestForCanvasApiHandle _handler;

    [SetUp]
    public void SetUp()
    {
        _mockEnrollmentsService = new Mock<IEnrollmentsService>();
        _handler = new GetTestForCanvasApiHandle(_mockEnrollmentsService.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnFailure_WhenValidationFails()
    {
        // Arrange
        var query = new GetTestForCanvasApiQuery { CourseId = "" }; // Invalid query
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("CourseId es requerido", result.Error); // Example error message
    }

    [Test]
    public async Task Handle_ShouldReturnSuccess_WhenServiceReturnsValidData()
    {
        // Arrange
        var query = new GetTestForCanvasApiQuery { CourseId = "123" };
        var enrollmentData = new List<gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models.Enrollment>
        {
            new gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models.Enrollment
            {
                Id = 1,
                UserId = 101,
                CourseId = 123,
                Type = "Student",
                Grades = new gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models.Grades
                {
                    CurrentGrade = 90,
                    FinalGrade = 95,
                    CurrentScore = 88,
                    HtmlUrl = "http://example.com",
                    FinalScore = 92
                },
                User = new gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models.User
                {
                    Id = 101,
                    Name = "John Doe"
                }
            }
        };

        _mockEnrollmentsService
            .Setup(s => s.GetStudentsByCourseAsync("123"))
            .ReturnsAsync(Result.Success(enrollmentData));

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(1, result.Value.Enrollments.Count);

        var enrollment = result.Value.Enrollments.First();
        Assert.AreEqual(1, enrollment.Id);
        Assert.AreEqual("Student", enrollment.Type);
        Assert.AreEqual(123, enrollment.CourseId);
        Assert.AreEqual("John Doe", enrollment.User.Name);
        Assert.AreEqual(90, enrollment.Grades.CurrentGrade);
        Assert.AreEqual(95, enrollment.Grades.FinalGrade);
        Assert.AreEqual(88, enrollment.Grades.CurrentScore);
        Assert.AreEqual(92, enrollment.Grades.FinalScore);
        Assert.AreEqual("http://example.com", enrollment.Grades.HtmlUrl);
    }

    [Test]
    public async Task Handle_ShouldReturnFailure_WhenServiceFails()
    {
        // Arrange
        var query = new GetTestForCanvasApiQuery { CourseId = "123" };

        _mockEnrollmentsService
            .Setup(s => s.GetStudentsByCourseAsync("123"))
            .ReturnsAsync(Result.Failure<List<gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models.Enrollment>>("Service error"));

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Service error", result.Error);
    }

    [Test]
    public async Task Handle_ShouldTransformDataCorrectly_WhenServiceReturnsData()
    {
        // Arrange
        var query = new GetTestForCanvasApiQuery { CourseId = "123" };
        var enrollmentData = new List<Enrollment>
        {
            new Enrollment
            {
                Id = 1,
                UserId = 101,
                CourseId = 123,
                Type = "Student",
                Grades = new Grades
                {
                    CurrentGrade = 85,
                    FinalGrade = 90,
                    CurrentScore = 80,
                    HtmlUrl = "http://example.com",
                    FinalScore = 88
                },
                User = new User { Id = 101, Name = "Jane Doe" }
            }
        };

        _mockEnrollmentsService
            .Setup(s => s.GetStudentsByCourseAsync("123"))
            .ReturnsAsync(Result.Success(enrollmentData.Select(e => new gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models.Enrollment
            {
                // Map properties from the source Enrollment type to the destination Enrollment type
                Id = e.Id,
                UserId = e.UserId,
                CourseId = e.CourseId,
                Type = e.Type,
                Grades = new gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models.Grades
                {
                    HtmlUrl = e.Grades.HtmlUrl,
                    CurrentGrade = e.Grades.CurrentGrade,
                    CurrentScore = e.Grades.CurrentScore,
                    FinalGrade = e.Grades.FinalGrade,
                    FinalScore = e.Grades.FinalScore
                },
                User = new gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models.User
                {
                    Id = e.User.Id,
                    Name = e.User.Name
                }
            }).ToList()));

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        var enrollment = result.Value.Enrollments.First();
        Assert.AreEqual(1, enrollment.Id);
        Assert.AreEqual("Jane Doe", enrollment.User.Name);
        Assert.AreEqual(85, enrollment.Grades.CurrentGrade);
    }
}