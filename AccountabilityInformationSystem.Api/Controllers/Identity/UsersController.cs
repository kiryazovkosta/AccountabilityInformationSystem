using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Models.Identity.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Controllers.Identity;

[ApiController]
[Route("api/identity/users")]
[Authorize]
public sealed class UsersController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUserById(string id)
    {
        UserResponse? userResponse = await dbContext.Users
            .Where(user => user.Id == id)
            .Select(UserQueries.ProjectToResponse())
            .FirstOrDefaultAsync();

        if (userResponse is null)
        {
            return NotFound();
        }

        return Ok(userResponse);
    }
}
