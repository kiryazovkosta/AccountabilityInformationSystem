using Microsoft.AspNetCore.Diagnostics;

namespace AccountabilityInformationSystem.Api.Middleware;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
        => problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new()
                {
                    Title = "Internal Server Error",
                    Detail = environment.IsDevelopment()
                        ? $"{exception.GetType().Name}: {exception.Message}"
                        : "An error occurred while processing your request. Please try again.",
                    Status = StatusCodes.Status500InternalServerError
                }
            });
}
