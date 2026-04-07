namespace AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

public sealed class Result<TResult>
    where TResult : notnull
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public List<Error>? Errors { get; }

    public TResult? Value
    {
        get
        {
            if (IsFailure || field is null)
            {
                throw new InvalidOperationException(
                    "Cannot access Value on a failed Result. Check IsSuccess before accessing Value.");
            }

            return field;
        }
        private set => field = value;
    }

    private Result(bool isSuccess, TResult? value, List<Error>? errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
    }

    public static Result<TResult> Success(TResult value) => new(true,  value,   null);
    public static Result<TResult> Failed(List<Error> errors) => new(false, default, errors);
}
