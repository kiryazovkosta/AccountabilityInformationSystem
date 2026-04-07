namespace AccountabilityInformationSystem.Api;

public static class ErrorCodes
{
    public const string NotFound = "NotFound";
    public const string BadRequest = "BadRequest";
    public const string Validation = "Validation";
    public const string Forbidden = "Forbidden";
    public const string Unauthorized = "Unauthorized";
    public const string Conflict = "Conflict";
}

public sealed record ErrorData(int Status, string Title, string Type)
{
    private const string FallbackType   = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
    private const string FallbackTitle  = "Bad Request";
    private const int    FallbackStatus = StatusCodes.Status400BadRequest;

    public static ErrorData Default => new ErrorData(FallbackStatus, FallbackTitle, FallbackType);
}

public static class ErrorMapping
{
    public static IReadOnlyDictionary<string, ErrorData> Map { get; } =
        new Dictionary<string, ErrorData>
        {
            [ErrorCodes.NotFound] = new(404, "Not Found", "https://tools.ietf.org/html/rfc9110#section-15.5.5"),
            [ErrorCodes.BadRequest] = new(400, "Bad Request", "https://tools.ietf.org/html/rfc9110#section-15.5.1"),
            [ErrorCodes.Validation] = new(400, "Validation Error", "https://tools.ietf.org/html/rfc9110#section-15.5.1"),
            [ErrorCodes.Forbidden] = new(403, "Forbidden", "https://tools.ietf.org/html/rfc9110#section-15.5.4"),
            [ErrorCodes.Unauthorized] = new(401, "Unauthorized", "https://tools.ietf.org/html/rfc9110#section-15.5.2"),
            [ErrorCodes.Conflict] = new(409, "Conflict", "https://tools.ietf.org/html/rfc9110#section-15.5.10"),
        };
}
