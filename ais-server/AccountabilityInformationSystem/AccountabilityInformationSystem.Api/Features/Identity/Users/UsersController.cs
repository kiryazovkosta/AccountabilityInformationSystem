using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Users.GetById;
using AccountabilityInformationSystem.Api.Features.Identity.Users.GetCurrent;
using AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace AccountabilityInformationSystem.Api.Features.Identity.Users;

[Route("api/identity/users")]
[Authorize]
public sealed class UsersController(IMessageBus bus) : ApiController
{
    [HttpGet("{id}")]
    [Authorize(Roles = $"{Role.Admin}")]
    public async Task<IActionResult> GetUserById(string id, CancellationToken cancellationToken)
    {
        Result<UserResponse> result = await bus.InvokeAsync<Result<UserResponse>>(
            new GetUserByIdRequest(id), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        Result<UserResponse> result = await bus.InvokeAsync<Result<UserResponse>>(
            new GetCurrentUserRequest(), cancellationToken);
        return result.ToActionResult();
    }
}
