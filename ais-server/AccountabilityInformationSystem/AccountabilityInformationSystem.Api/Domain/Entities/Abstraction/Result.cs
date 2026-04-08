namespace AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

public enum ResultSuccessType
{
    Ok = 200,
    Created = 201,
    Accepted = 202,
    NoContent= 204
}

public enum ResultFailureType
{
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    Conflict = 409,
    Validation = 422,
    InternalServerError = 500
}

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ResultSuccessType? SuccessType { get; }
    public ResultFailureType? FailureType { get; }
    public Error? Error { get; }

    protected Result(ResultSuccessType successType)
    {
        IsSuccess = true;
        SuccessType = successType;
    }

    protected Result(Error error, ResultFailureType failureType)
    {
        IsSuccess = false;
        FailureType = failureType;
        Error = error;
    }

    public static Result Success(ResultSuccessType successType = ResultSuccessType.Ok) 
        => new(successType);
    public static Result Failure(Error error, ResultFailureType failureType = ResultFailureType.BadRequest) 
        => new(error, failureType);
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value, ResultSuccessType successType) : base(successType)
    {
        Value = value;
    }

    private Result(Error error, ResultFailureType failureType) : base(error, failureType) { }

    public static Result<T> Success(T value, ResultSuccessType successType = ResultSuccessType.Ok) 
        => new(value, successType);
    public static new Result<T> Failure(Error error, ResultFailureType failureType = ResultFailureType.BadRequest) 
        => new(error, failureType);
}
