using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Shared.Factories;

public static class ErrorResponseFactory
{
    public static IActionResult Create()
    {
        //if (errors is null || errors.Count == 0)
        //{
        //    throw new ArgumentException("At least one error is required.", nameof(errors));
        //}

        //string dominantCode = errors[0].Code;
        //ErrorData errorData = ErrorMapping.Map.TryGetValue(dominantCode, out ErrorData? data)
        //    ? data
        //    : ErrorData.Default;

        //Dictionary<string, object?> errorExtensions = new()
        //{
        //    { "errors", errors.GroupBy(e => e.Code)
        //        .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray()) }
        //};

        ProblemDetails problemDetails = new()
        {
            Status = 400,
            Title = "some title",
            Type = "some type",
        };

        //return new ObjectResult(problemDetails) { StatusCode = errorData.Status };

        return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
    }
}
