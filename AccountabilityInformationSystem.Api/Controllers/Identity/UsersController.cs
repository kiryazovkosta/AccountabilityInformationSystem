using System.Security.Claims;
using System.Threading;
using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities.Identity;
using AccountabilityInformationSystem.Api.Models.Identity.Users;
using AccountabilityInformationSystem.Api.Services.UserContexting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Controllers.Identity;

[ApiController]
[Route("api/identity/users")]
[Authorize]
public sealed class UsersController(UserContext userContext) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUserById(
        string id, CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
        }

        if (user.Id != id)
        {
            return Problem(statusCode: StatusCodes.Status403Forbidden, detail: "Access to this resource is forbidden");
        }

        UserResponse userResponse = user.ToResponse();
        return Ok(userResponse);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser(CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
        }

        UserResponse? userResponse = user.ToResponse();
        return Ok(userResponse);
    }
}
