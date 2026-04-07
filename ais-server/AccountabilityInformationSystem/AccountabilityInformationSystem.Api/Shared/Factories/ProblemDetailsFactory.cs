using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api;

public static class ErrorResponseFactory
{
    public static IActionResult Create(List<Error> errors)
    {
        if (errors is null || errors.Count == 0)
        {
            throw new ArgumentException("At least one error is required.", nameof(errors));
        }

        string dominantCode = errors[0].Code;
        ErrorData errorData = ErrorMapping.Map.TryGetValue(dominantCode, out ErrorData? data)
            ? data
            : ErrorData.Default;

        Dictionary<string, object?> errorExtensions = new()
        {
            { "errors", errors.ToDictionary(e => e.Code, e => e.Message) }
        };

        ProblemDetails problemDetails = new()
        {
            Status = errorData.Status,
            Title = errorData.Title,
            Type = errorData.Type,
            Extensions = errorExtensions
        };

        return new ObjectResult(problemDetails) { StatusCode = errorData.Status };
    }
}