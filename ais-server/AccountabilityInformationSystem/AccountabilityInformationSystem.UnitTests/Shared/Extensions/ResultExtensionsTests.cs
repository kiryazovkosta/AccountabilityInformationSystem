using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.UnitTests.Shared.Extensions;

public class ResultExtensionsTests
{
    private readonly Error _error = new("ErrorCode", "ErrorMessage");

    #region ToActionResult (non-generic)

    [Fact]
    public void ToActionResult_ShouldThrowArgumentNullException_WhenResultIsNull()
    {
        // Arrange
        Result result = null!;

        // Act && Assert
        Assert.Throws<ArgumentNullException>(() => result.ToActionResult());
    }

    [Theory]
    [InlineData(ResultFailureType.BadRequest, 400)]
    [InlineData(ResultFailureType.Unauthorized, 401)]
    [InlineData(ResultFailureType.Forbidden, 403)]
    [InlineData(ResultFailureType.NotFound, 404)]
    [InlineData(ResultFailureType.Conflict, 409)]
    [InlineData(ResultFailureType.Validation, 422)]
    [InlineData(ResultFailureType.InternalServerError, 500)]
    public void ToActionResult_ShouldReturnCorrectStatusCode_WhenResultIsFailure(ResultFailureType failureType, int expectedStatus)
    {
        // Arrange
        Result result = Result.Failure(_error, failureType);

        // Act
        IActionResult actionResult = result.ToActionResult();

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(expectedStatus, objectResult.StatusCode);
    }

    [Theory]
    [InlineData(ResultFailureType.BadRequest, 400, "Bad Request", "https://tools.ietf.org/html/rfc9110#section-15.5.1")]
    [InlineData(ResultFailureType.Unauthorized, 401, "Unauthorized", "https://tools.ietf.org/html/rfc9110#section-15.5.2")]
    [InlineData(ResultFailureType.Forbidden, 403, "Forbidden", "https://tools.ietf.org/html/rfc9110#section-15.5.4")]
    [InlineData(ResultFailureType.NotFound, 404, "Not Found", "https://tools.ietf.org/html/rfc9110#section-15.5.5")]
    [InlineData(ResultFailureType.Conflict, 409, "Conflict", "https://tools.ietf.org/html/rfc9110#section-15.5.10")]
    [InlineData(ResultFailureType.Validation, 422, "Unprocessable Content", "https://tools.ietf.org/html/rfc9110#section-15.5.21")]
    [InlineData(ResultFailureType.InternalServerError, 500, "Internal Server Error", "https://tools.ietf.org/html/rfc9110#section-15.6.1")]
    public void ToActionResult_ShouldReturnProblemDetailsWithCorrectFields_WhenResultIsFailure(
        ResultFailureType failureType, int expectedStatus, string expectedTitle, string expectedType)
    {
        // Arrange
        Result result = Result.Failure(_error, failureType);

        // Act
        IActionResult actionResult = result.ToActionResult();

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(actionResult);
        ProblemDetails problem = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(expectedStatus, problem.Status);
        Assert.Equal(expectedTitle, problem.Title);
        Assert.Equal(expectedType, problem.Type);
    }

    [Fact]
    public void ToActionResult_ShouldIncludeErrorsInExtensions_WhenResultIsFailure()
    {
        // Arrange
        Result result = Result.Failure(_error);

        // Act
        IActionResult actionResult = result.ToActionResult();

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(actionResult);
        ProblemDetails problem = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.True(problem.Extensions.ContainsKey("errors"));
        Dictionary<string, string[]> errors = Assert.IsType<Dictionary<string, string[]>>(problem.Extensions["errors"]);
        Assert.True(errors.ContainsKey(_error.Code));
        Assert.Contains(_error.Message, errors[_error.Code]);
    }

    [Fact]
    public void ToActionResult_ShouldGroupErrorsByCode_WhenMultipleErrorsShareTheSameCode()
    {
        // Arrange
        Error secondError = new(_error.Code, "AnotherMessage");
        Result result = Result.Failure([_error, secondError]);

        // Act
        IActionResult actionResult = result.ToActionResult();

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(actionResult);
        ProblemDetails problem = Assert.IsType<ProblemDetails>(objectResult.Value);
        Dictionary<string, string[]> errors = Assert.IsType<Dictionary<string, string[]>>(problem.Extensions["errors"]);
        Assert.Single(errors);
        Assert.Equal(2, errors[_error.Code].Length);
        Assert.Contains(_error.Message, errors[_error.Code]);
        Assert.Contains(secondError.Message, errors[_error.Code]);
    }

    [Fact]
    public void ToActionResult_ShouldGroupErrorsByCode_WhenErrorsHaveDifferentCodes()
    {
        // Arrange
        Error secondError = new("AnotherCode", "AnotherMessage");
        Result result = Result.Failure([_error, secondError]);

        // Act
        IActionResult actionResult = result.ToActionResult();

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(actionResult);
        ProblemDetails problem = Assert.IsType<ProblemDetails>(objectResult.Value);
        Dictionary<string, string[]> errors = Assert.IsType<Dictionary<string, string[]>>(problem.Extensions["errors"]);
        Assert.Equal(2, errors.Count);
        Assert.Contains(_error.Message, errors[_error.Code]);
        Assert.Contains(secondError.Message, errors[secondError.Code]);
    }

    [Fact]
    public void ToActionResult_ShouldReturnOkResult_WhenResultIsSuccessWithOkType()
    {
        // Arrange
        Result result = Result.Success(ResultSuccessType.Ok);

        // Act
        IActionResult actionResult = result.ToActionResult();

        // Assert
        Assert.IsType<OkResult>(actionResult);
    }

    [Fact]
    public void ToActionResult_ShouldReturnStatusCode201_WhenResultIsSuccessWithCreatedType()
    {
        // Arrange
        Result result = Result.Success(ResultSuccessType.Created);

        // Act
        IActionResult actionResult = result.ToActionResult();

        // Assert
        StatusCodeResult statusCodeResult = Assert.IsType<StatusCodeResult>(actionResult);
        Assert.Equal(201, statusCodeResult.StatusCode);
    }

    [Fact]
    public void ToActionResult_ShouldReturnStatusCode202_WhenResultIsSuccessWithAcceptedType()
    {
        // Arrange
        Result result = Result.Success(ResultSuccessType.Accepted);

        // Act
        IActionResult actionResult = result.ToActionResult();

        // Assert
        StatusCodeResult statusCodeResult = Assert.IsType<StatusCodeResult>(actionResult);
        Assert.Equal(202, statusCodeResult.StatusCode);
    }

    [Fact]
    public void ToActionResult_ShouldReturnNoContentResult_WhenResultIsSuccessWithNoContentType()
    {
        // Arrange
        Result result = Result.Success(ResultSuccessType.NoContent);

        // Act
        IActionResult actionResult = result.ToActionResult();

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    #endregion

    #region ToActionResult<T> (generic)

    [Fact]
    public void ToActionResult_Generic_ShouldReturnProblemDetails_WhenResultIsFailure()
    {
        // Arrange
        Result<string> result = Result<string>.Failure(_error, ResultFailureType.NotFound);

        // Act
        IActionResult actionResult = result.ToActionResult();

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(actionResult);
        ProblemDetails problem = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(404, problem.Status);
        Assert.Equal("Not Found", problem.Title);
    }

    [Fact]
    public void ToActionResult_Generic_ShouldReturnOkObjectResult_WhenResultIsSuccessWithOkType()
    {
        // Arrange
        const string value = "test-value";
        Result<string> result = Result<string>.Success(value, ResultSuccessType.Ok);

        // Act
        IActionResult actionResult = result.ToActionResult();

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(value, okResult.Value);
    }

    [Fact]
    public void ToActionResult_Generic_ShouldReturnObjectResultWith201_WhenResultIsSuccessWithCreatedType()
    {
        // Arrange
        const string value = "test-value";
        Result<string> result = Result<string>.Success(value, ResultSuccessType.Created);

        // Act
        IActionResult actionResult = result.ToActionResult();

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(201, objectResult.StatusCode);
        Assert.Equal(value, objectResult.Value);
    }

    [Fact]
    public void ToActionResult_Generic_ShouldReturnObjectResultWith202_WhenResultIsSuccessWithAcceptedType()
    {
        // Arrange
        const string value = "test-value";
        Result<string> result = Result<string>.Success(value, ResultSuccessType.Accepted);

        // Act
        IActionResult actionResult = result.ToActionResult();

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(202, objectResult.StatusCode);
        Assert.Equal(value, objectResult.Value);
    }

    [Fact]
    public void ToActionResult_Generic_ShouldReturnNoContentResult_WhenResultIsSuccessWithNoContentType()
    {
        // Arrange
        Result<string> result = Result<string>.Success("test-value", ResultSuccessType.NoContent);

        // Act
        IActionResult actionResult = result.ToActionResult();

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    #endregion

    #region IsSuccessWith

    [Theory]
    [InlineData(ResultSuccessType.Ok)]
    [InlineData(ResultSuccessType.Created)]
    [InlineData(ResultSuccessType.Accepted)]
    [InlineData(ResultSuccessType.NoContent)]
    public void IsSuccessWith_ShouldReturnTrue_WhenResultIsSuccessAndTypeMatches(ResultSuccessType successType)
    {
        // Arrange
        Result result = Result.Success(successType);

        // Act
        bool isSuccessWith = result.IsSuccessWith(successType);

        // Assert
        Assert.True(isSuccessWith);
    }

    [Theory]
    [InlineData(ResultSuccessType.Ok, ResultSuccessType.Created)]
    [InlineData(ResultSuccessType.Created, ResultSuccessType.Accepted)]
    [InlineData(ResultSuccessType.Accepted, ResultSuccessType.NoContent)]
    [InlineData(ResultSuccessType.NoContent, ResultSuccessType.Ok)]
    public void IsSuccessWith_ShouldReturnFalse_WhenResultIsSuccessButTypeDoesNotMatch(ResultSuccessType actual, ResultSuccessType queried)
    {
        // Arrange
        Result result = Result.Success(actual);

        // Act
        bool isSuccessWith = result.IsSuccessWith(queried);

        // Assert
        Assert.False(isSuccessWith);
    }

    [Theory]
    [InlineData(ResultSuccessType.Ok)]
    [InlineData(ResultSuccessType.Created)]
    [InlineData(ResultSuccessType.Accepted)]
    [InlineData(ResultSuccessType.NoContent)]
    public void IsSuccessWith_ShouldReturnFalse_WhenResultIsFailure(ResultSuccessType successType)
    {
        // Arrange
        Result result = Result.Failure(_error);

        // Act
        bool isSuccessWith = result.IsSuccessWith(successType);

        // Assert
        Assert.False(isSuccessWith);
    }

    #endregion

    #region IsFailureWith

    [Theory]
    [InlineData(ResultFailureType.BadRequest)]
    [InlineData(ResultFailureType.Unauthorized)]
    [InlineData(ResultFailureType.Forbidden)]
    [InlineData(ResultFailureType.NotFound)]
    [InlineData(ResultFailureType.Conflict)]
    [InlineData(ResultFailureType.Validation)]
    [InlineData(ResultFailureType.InternalServerError)]
    public void IsFailureWith_ShouldReturnTrue_WhenResultIsFailureAndTypeMatches(ResultFailureType failureType)
    {
        // Arrange
        Result result = Result.Failure(_error, failureType);

        // Act
        bool isFailureWith = result.IsFailureWith(failureType);

        // Assert
        Assert.True(isFailureWith);
    }

    [Theory]
    [InlineData(ResultFailureType.BadRequest, ResultFailureType.NotFound)]
    [InlineData(ResultFailureType.Unauthorized, ResultFailureType.Forbidden)]
    [InlineData(ResultFailureType.Forbidden, ResultFailureType.Conflict)]
    [InlineData(ResultFailureType.NotFound, ResultFailureType.Validation)]
    [InlineData(ResultFailureType.Conflict, ResultFailureType.InternalServerError)]
    [InlineData(ResultFailureType.Validation, ResultFailureType.BadRequest)]
    [InlineData(ResultFailureType.InternalServerError, ResultFailureType.Unauthorized)]
    public void IsFailureWith_ShouldReturnFalse_WhenResultIsFailureButTypeDoesNotMatch(ResultFailureType actual, ResultFailureType queried)
    {
        // Arrange
        Result result = Result.Failure(_error, actual);

        // Act
        bool isFailureWith = result.IsFailureWith(queried);

        // Assert
        Assert.False(isFailureWith);
    }

    [Theory]
    [InlineData(ResultFailureType.BadRequest)]
    [InlineData(ResultFailureType.Unauthorized)]
    [InlineData(ResultFailureType.Forbidden)]
    [InlineData(ResultFailureType.NotFound)]
    [InlineData(ResultFailureType.Conflict)]
    [InlineData(ResultFailureType.Validation)]
    [InlineData(ResultFailureType.InternalServerError)]
    public void IsFailureWith_ShouldReturnFalse_WhenResultIsSuccess(ResultFailureType failureType)
    {
        // Arrange
        Result result = Result.Success();

        // Act
        bool isFailureWith = result.IsFailureWith(failureType);

        // Assert
        Assert.False(isFailureWith);
    }

    #endregion
}
