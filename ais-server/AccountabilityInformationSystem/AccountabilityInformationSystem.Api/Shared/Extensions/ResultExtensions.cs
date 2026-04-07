using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Shared.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToResult<TResult>(this Result<TResult> result)
        where TResult : notnull
    {
        if (result.IsFailure && result.Errors is { Count: > 0 })
        {
            return ErrorResponseFactory.Create(result.Errors);
        }

        return new OkObjectResult(result.Value);
    }

    public static IActionResult ToCreatedResult<TResult>(
        this Result<TResult> result,
        string actionName,
        object? routeValues)
        where TResult : notnull
    {
        if (result.IsFailure && result.Errors is { Count: > 0 })
        {
            return ErrorResponseFactory.Create(result.Errors);
        }

        return new CreatedAtActionResult(actionName, controllerName: null, routeValues, result.Value);
    }

    public static IActionResult ToNoContentResult<TResult>(this Result<TResult> result)
        where TResult : notnull
    {
        if (result.IsFailure && result.Errors is { Count: > 0 })
        {
            return ErrorResponseFactory.Create(result.Errors);
        }

        return new NoContentResult();
    }

    public static IActionResult ToAcceptedResult<TResult>(this Result<TResult> result)
        where TResult : notnull
    {
        if (result.IsFailure && result.Errors is { Count: > 0 })
        {
            return ErrorResponseFactory.Create(result.Errors);
        }

        return new AcceptedResult();
    }
}
