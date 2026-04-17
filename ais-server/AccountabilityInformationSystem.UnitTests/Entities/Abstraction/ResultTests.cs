using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using System.Collections.Generic;

namespace AccountabilityInformationSystem.UnitTests.Entities.Abstraction;

public class ResultTests
{
    private readonly Error error = new("ErrorCode", "ErrorMessage");

    public readonly bool success = true;

    [Fact]
    public void Validate_ShouldSuccess_WhenCreateASuccessResult()
    {
        //Arrange && Act
        Result successResult = Result.Success();

        // Assert
        Assert.True(successResult.IsSuccess);
        Assert.False(successResult.IsFailure);
        Assert.Equal(ResultSuccessType.Ok, successResult.SuccessType);
        Assert.Null(successResult.FailureType);
        Assert.Empty(successResult.Errors);
    }

    [Theory]
    [InlineData(ResultSuccessType.Ok, ResultSuccessType.Ok)]
    [InlineData(ResultSuccessType.Created, ResultSuccessType.Created)]
    [InlineData(ResultSuccessType.Accepted, ResultSuccessType.Accepted)]
    [InlineData(ResultSuccessType.NoContent, ResultSuccessType.NoContent)]
    public void Validate_ShouldSuccess_WhenCreateASuccessResultWithSpecificSuccessType(ResultSuccessType created, ResultSuccessType expected)
    {
        //Arrange && Act
        Result successResult = Result.Success(created);

        // Assert
        Assert.True(successResult.IsSuccess);
        Assert.False(successResult.IsFailure);
        Assert.Equal(expected, successResult.SuccessType);
        Assert.Null(successResult.FailureType);
        Assert.Empty(successResult.Errors);
    }

    [Fact]
    public void Validate_ShouldSuccess_WhenCreateAFailureResult()
    {
        //Arrange && Act
        Error newError = new("Code", "New error message");
        Result failureResult = Result.Failure([error, newError]);

        // Assert
        Assert.True(failureResult.IsFailure);
        Assert.False(failureResult.IsSuccess);
        Assert.Equal(ResultFailureType.BadRequest, failureResult.FailureType);
        Assert.NotEmpty(failureResult.Errors);
        Assert.Equal(2, failureResult.Errors.Count);
        Assert.Equal(error.Code, failureResult.Errors[0].Code);
        Assert.Equal(error.Message, failureResult.Errors[0].Message);
        Assert.Equal(newError.Code, failureResult.Errors[1].Code);
        Assert.Equal(newError.Message, failureResult.Errors[1].Message);
        Assert.Null(failureResult.SuccessType);
    }

    [Theory]
    [InlineData(ResultFailureType.BadRequest, ResultFailureType.BadRequest)]
    [InlineData(ResultFailureType.Unauthorized, ResultFailureType.Unauthorized)]
    [InlineData(ResultFailureType.Forbidden, ResultFailureType.Forbidden)]
    [InlineData(ResultFailureType.NotFound, ResultFailureType.NotFound)]
    [InlineData(ResultFailureType.Conflict, ResultFailureType.Conflict)]
    [InlineData(ResultFailureType.Validation, ResultFailureType.Validation)]
    [InlineData(ResultFailureType.InternalServerError, ResultFailureType.InternalServerError)]
    public void Validate_ShouldSuccess_WhenCreateAFailureResultWithSpecificFailureType(ResultFailureType failure, ResultFailureType expected)
    {
        //Arrange && Act
        Result failureResult = Result.Failure(error, failure);

        // Assert
        Assert.True(failureResult.IsFailure);
        Assert.False(failureResult.IsSuccess);
        Assert.Equal(expected, failureResult.FailureType);
        Assert.NotEmpty(failureResult.Errors);
        Assert.Equal(error.Code, failureResult.Errors[0].Code);
        Assert.Equal(error.Message, failureResult.Errors[0].Message);
        Assert.Null(failureResult.SuccessType);
    }

    [Fact]
    public void Validate_ShouldSuccess_WhenCreateAFailureResultWithErrorsCollection()
    {
        //Arrange && Act
        Result failureResult = Result.Failure(error);

        // Assert
        Assert.True(failureResult.IsFailure);
        Assert.False(failureResult.IsSuccess);
        Assert.Equal(ResultFailureType.BadRequest, failureResult.FailureType);
        Assert.NotEmpty(failureResult.Errors);
        Assert.Equal(error.Code, failureResult.Errors[0].Code);
        Assert.Equal(error.Message, failureResult.Errors[0].Message);
        Assert.Null(failureResult.SuccessType);
    }

    [Fact]
    public void Validate_ShouldSuccess_WhenCreateASuccessGenericResult()
    {
        //Arrange && Act
        Result<bool> successResult = Result<bool>.Success(success);

        // Assert
        Assert.True(successResult.IsSuccess);
        Assert.False(successResult.IsFailure);
        Assert.Equal(ResultSuccessType.Ok, successResult.SuccessType);
        Assert.True(successResult.Value);
        Assert.Null(successResult.FailureType);
        Assert.Empty(successResult.Errors);
    }

    [Theory]
    [InlineData(ResultSuccessType.Ok, ResultSuccessType.Ok)]
    [InlineData(ResultSuccessType.Created, ResultSuccessType.Created)]
    [InlineData(ResultSuccessType.Accepted, ResultSuccessType.Accepted)]
    [InlineData(ResultSuccessType.NoContent, ResultSuccessType.NoContent)]
    public void Validate_ShouldSuccess_WhenCreateASuccessGenericResultWithSpecificSuccessType(ResultSuccessType created, ResultSuccessType expected)
    {
        //Arrange && Act
        Result<string> successResult = Result<string>.Success("Some message", created);

        // Assert
        Assert.True(successResult.IsSuccess);
        Assert.False(successResult.IsFailure);
        Assert.Equal(expected, successResult.SuccessType);
        Assert.NotNull(successResult.Value);
        Assert.Equal("Some message", successResult.Value);
        Assert.Null(successResult.FailureType);
        Assert.Empty(successResult.Errors);
    }

    [Fact]
    public void Validate_ShouldSuccess_WhenCreateAFailureGenericResult()
    {
        //Arrange && Act
        Result failureResult = Result<bool>.Failure(error);

        // Assert
        Assert.True(failureResult.IsFailure);
        Assert.False(failureResult.IsSuccess);
        Assert.Equal(ResultFailureType.BadRequest, failureResult.FailureType);
        Assert.NotEmpty(failureResult.Errors);
        Assert.Equal(error.Code, failureResult.Errors[0].Code);
        Assert.Equal(error.Message, failureResult.Errors[0].Message);
        Assert.Null(failureResult.SuccessType);
    }

    [Theory]
    [InlineData(ResultFailureType.BadRequest, ResultFailureType.BadRequest)]
    [InlineData(ResultFailureType.Unauthorized, ResultFailureType.Unauthorized)]
    [InlineData(ResultFailureType.Forbidden, ResultFailureType.Forbidden)]
    [InlineData(ResultFailureType.NotFound, ResultFailureType.NotFound)]
    [InlineData(ResultFailureType.Conflict, ResultFailureType.Conflict)]
    [InlineData(ResultFailureType.Validation, ResultFailureType.Validation)]
    [InlineData(ResultFailureType.InternalServerError, ResultFailureType.InternalServerError)]
    public void Validate_ShouldSuccess_WhenCreateAFailureGenericResultWithSpecificFailureType(ResultFailureType failure, ResultFailureType expected)
    {
        //Arrange && Act

        Result<string> failureResult = Result<string>.Failure(error, failure);

        // Assert
        Assert.True(failureResult.IsFailure);
        Assert.False(failureResult.IsSuccess);
        Assert.Equal(expected, failureResult.FailureType);
        Assert.NotEmpty(failureResult.Errors);
        Assert.Equal(error.Code, failureResult.Errors[0].Code);
        Assert.Equal(error.Message, failureResult.Errors[0].Message);
        Assert.Null(failureResult.Value);
        Assert.Null(failureResult.SuccessType);
    }

    [Fact]
    public void Validate_ShouldSuccess_WhenCreateAFailureGenericResultWithErrorsCollection()
    {
        //Arrange && Act
        Error newError = new("Code", "New error message");
        Result failureResult = Result<bool>.Failure([error, newError]);

        // Assert
        Assert.True(failureResult.IsFailure);
        Assert.False(failureResult.IsSuccess);
        Assert.Equal(ResultFailureType.BadRequest, failureResult.FailureType);
        Assert.NotEmpty(failureResult.Errors);
        Assert.Equal(2, failureResult.Errors.Count);
        Assert.Equal(error.Code, failureResult.Errors[0].Code);
        Assert.Equal(error.Message, failureResult.Errors[0].Message);
        Assert.Equal(newError.Code, failureResult.Errors[1].Code);
        Assert.Equal(newError.Message, failureResult.Errors[1].Message);
        Assert.Null(failureResult.SuccessType);
    }
}
