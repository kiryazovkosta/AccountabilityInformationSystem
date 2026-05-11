using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Identity.Users;

[Route("api/identity/users")]
[Authorize]
public sealed class UsersController(UserContext userContext, ApplicationDbContext dbContext) : ApiController
{
    [HttpGet("{id}")]
    [Authorize(Roles = $"{Role.Admin}")]
    public async Task<ActionResult<UserResponse>> GetUserById(
        string id, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: "User not found");
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
