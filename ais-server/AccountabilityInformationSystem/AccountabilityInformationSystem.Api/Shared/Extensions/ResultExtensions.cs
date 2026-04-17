using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Shared.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsFailure && result.Errors.Count > 0)
        {
            ResultFailureType failureType = result.FailureType!.Value;
            if (!ErrorMapping.Map.TryGetValue(failureType, out ErrorData? errorData))
            {
                errorData = ErrorData.Default;
            }

            Dictionary<string, object?> extensions = new()
            {
                { "errors", result.Errors.GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray()) }
            };

            ProblemDetails problem = new()
            {
                Type = errorData.Type,
                Title = errorData.Title,
                Status = errorData.Status,
                Extensions = extensions
            };
            return new ObjectResult(problem) { StatusCode = problem.Status };
        }

        return result.SuccessType switch
        {
            ResultSuccessType.Created => new StatusCodeResult(201),
            ResultSuccessType.Accepted => new StatusCodeResult(202),
            ResultSuccessType.NoContent => new NoContentResult(),
            _ => new OkResult()
        };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsFailure)
        {
            return ((Result)result).ToActionResult();
        }

        return result.SuccessType switch
        {
            ResultSuccessType.Created => new ObjectResult(result.Value) { StatusCode = 201 },
            ResultSuccessType.Accepted => new ObjectResult(result.Value) { StatusCode = 202 },
            ResultSuccessType.NoContent => new NoContentResult(),
            _ => new OkObjectResult(result.Value)
        };
    }

    public static bool IsSuccessWith(this Result result, ResultSuccessType type)
        => result.IsSuccess && result.SuccessType == type;

    public static bool IsFailureWith(this Result result, ResultFailureType type)
        => result.IsFailure && result.FailureType == type;
}
