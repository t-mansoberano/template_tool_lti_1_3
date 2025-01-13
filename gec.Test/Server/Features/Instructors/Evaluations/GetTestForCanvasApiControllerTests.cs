using CSharpFunctionalExtensions;
using gec.Application.Features.Instructors.Evaluations.Queries.GetTestForCanvasApi;
using gec.Server.Common;
using gec.Server.Features.Instructors.Evaluations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace gec.Server.Tests.Features.Instructors.Evaluations;

[TestFixture]
public class GetTestForCanvasApiControllerTests
{
    [SetUp]
    public void SetUp()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new GetTestForCanvasApiController(_mockMediator.Object);
    }

    private Mock<IMediator> _mockMediator;
    private GetTestForCanvasApiController _controller;

    [Test]
    public async Task GetStudentsByCourseAsync_ShouldReturnError_WhenMediatorFails()
    {
        // Arrange
        var courseId = "123";
        var query = new GetTestForCanvasApiQuery();
        var errorMessage = "Course not found";

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetTestForCanvasApiQuery>(), default))
            .ReturnsAsync(Result.Failure<GetTestForCanvasApiRespond>(errorMessage));

        // Act
        var result = await _controller.GetStudentsByCourseAsync(courseId, query);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.AreEqual(400, objectResult.StatusCode);
        Assert.AreEqual(errorMessage, ((Envelope)objectResult.Value).ErrorMessage);
    }

    [Test]
    public async Task GetStudentsByCourseAsync_ShouldReturnOk_WhenMediatorSucceeds()
    {
        // Arrange
        var courseId = "123";
        var query = new GetTestForCanvasApiQuery();
        var expectedResponse = new GetTestForCanvasApiRespond
        {
            Enrollments = new List<Enrollment>
            {
                new()
                {
                    Id = 1,
                    UserId = 101,
                    CourseId = 123,
                    Type = "Student",
                    Grades = new Grades { CurrentGrade = 90, FinalGrade = 95 },
                    User = new User { Id = 101, Name = "John Doe" }
                }
            }
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetTestForCanvasApiQuery>(), default))
            .ReturnsAsync(Result.Success(expectedResponse));

        // Act
        var result = await _controller.GetStudentsByCourseAsync(courseId, query);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.AreSame(expectedResponse, ((Envelope<GetTestForCanvasApiRespond>)okResult.Value).Result);
    }
}