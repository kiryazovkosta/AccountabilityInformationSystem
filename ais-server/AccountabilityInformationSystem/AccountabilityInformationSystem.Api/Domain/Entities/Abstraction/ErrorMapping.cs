using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

namespace AccountabilityInformationSystem.Api;

public sealed record ErrorData(int Status, string Title, string Type)
{
    private const string FallbackType = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
    private const string FallbackTitle = "Bad Request";
    private const int FallbackStatus = StatusCodes.Status400BadRequest;

    public static ErrorData Default => new(FallbackStatus, FallbackTitle, FallbackType);
}

public static class ErrorMapping
{
    public static IReadOnlyDictionary<ResultFailureType, ErrorData> Map { get; } =
        new Dictionary<ResultFailureType, ErrorData>
        {
            [ResultFailureType.BadRequest] = new(400, "Bad Request", "https://tools.ietf.org/html/rfc9110#section-15.5.1"),
            [ResultFailureType.Unauthorized] = new(401, "Unauthorized", "https://tools.ietf.org/html/rfc9110#section-15.5.2"),
            [ResultFailureType.Forbidden] = new(403, "Forbidden", "https://tools.ietf.org/html/rfc9110#section-15.5.4"),
            [ResultFailureType.NotFound] = new(404, "Not Found", "https://tools.ietf.org/html/rfc9110#section-15.5.5"),
            [ResultFailureType.Conflict] = new(409, "Conflict", "https://tools.ietf.org/html/rfc9110#section-15.5.10"),
            [ResultFailureType.Validation] = new(422, "Unprocessable Content", "https://tools.ietf.org/html/rfc9110#section-15.5.21"),
            [ResultFailureType.InternalServerError] = new(500, "Internal Server Error", "https://tools.ietf.org/html/rfc9110#section-15.6.1"),
        };
}
