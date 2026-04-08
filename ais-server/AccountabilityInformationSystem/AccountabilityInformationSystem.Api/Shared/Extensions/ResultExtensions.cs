//using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
//using Microsoft.AspNetCore.Mvc;

//namespace AccountabilityInformationSystem.Api.Shared.Extensions;

//public static class ResultExtensions
//{
//    public static IActionResult ToResult<TResult>(this Result<TResult> result)
//        where TResult : notnull
//        => result.Match(
//            value => new OkObjectResult(value),
//            ErrorResponseFactory.Create);

//    public static IActionResult ToCreatedResult<TResult>(
//        this Result<TResult> result,
//        string actionName,
//        object? routeValues)
//        where TResult : notnull
//        => result.Match(
//            value => new CreatedAtActionResult(actionName, controllerName: null, routeValues, value),
//            ErrorResponseFactory.Create);

//    public static IActionResult ToNoContentResult<TResult>(this Result<TResult> result)
//        where TResult : notnull
//        => result.Match(
//            _ => new NoContentResult(),
//            ErrorResponseFactory.Create);

//    public static IActionResult ToAcceptedResult<TResult>(this Result<TResult> result)
//        where TResult : notnull
//        => result.Match(
//            value => new AcceptedResult()
//            ErrorResponseFactory.Create);
//}
