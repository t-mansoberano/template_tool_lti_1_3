using CSharpFunctionalExtensions;
using FluentAssertions;
using gec.Application.Features.Instructors.Evaluations.Queries.GetTestForCanvasApi;
using gec.Server.Common;
using gec.Server.Features.Instructors.Evaluations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace gec.Test.Server.Features.Instructors.Evaluations;

[TestFixture]
public class GetTestForCanvasApiControllerTests
{
    private Mock<IMediator> _mockMediator;
    private GetTestForCanvasApiController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new GetTestForCanvasApiController(_mockMediator.Object);
    }

    [Test]
    public async Task GetStudentsByCourseAsync_ShouldReturnError_WhenMediatorFails()
    {
        // Arrange
        const string courseId = TestConstants.VALID_COURSE_ID;
        var query = new GetTestForCanvasApiQuery();
        const string errorMessage = TestConstants.COURSE_NOT_FOUND_ERROR;

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetTestForCanvasApiQuery>(), default))
            .ReturnsAsync(Result.Failure<GetTestForCanvasApiRespond>(errorMessage));

        // Act
        var result = await _controller.GetStudentsByCourseAsync(courseId, query);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult.StatusCode.Should().Be(400);
        ((Envelope)objectResult.Value).ErrorMessage.Should().Be(errorMessage);

        // Verify that the mediator was called once
        _mockMediator.Verify(m => m.Send(It.IsAny<GetTestForCanvasApiQuery>(), default), Times.Once);
    }

    [Test]
    public async Task GetStudentsByCourseAsync_ShouldReturnOk_WhenMediatorSucceeds()
    {
        // Arrange
        const string courseId = TestConstants.VALID_COURSE_ID;
        var query = new GetTestForCanvasApiQuery();
        var expectedResponse = GetSampleResponse();

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetTestForCanvasApiQuery>(), default))
            .ReturnsAsync(Result.Success(expectedResponse));

        // Act
        var result = await _controller.GetStudentsByCourseAsync(courseId, query);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        ((Envelope<GetTestForCanvasApiRespond>)okResult.Value).Result.Should().BeEquivalentTo(expectedResponse);

        // Verify that the mediator was called once
        _mockMediator.Verify(m => m.Send(It.IsAny<GetTestForCanvasApiQuery>(), default), Times.Once);
    }

    private static GetTestForCanvasApiRespond GetSampleResponse()
    {
        return new GetTestForCanvasApiRespond
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
    }
}

public static class TestConstants
{
    public const string VALID_COURSE_ID = "123";
    public const string COURSE_NOT_FOUND_ERROR = "Course not found";
}
