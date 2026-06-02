using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Administration.Users.GetAll;
using AccountabilityInformationSystem.Api.Features.Identity.Users.GetById;
using AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace AccountabilityInformationSystem.Api.Features.Administration.Users;

[Route("api/admin/users")]
[Authorize(Roles = $"{Role.Admin}")]
public sealed class UsersController(IMessageBus bus) : ApiController
{
    [HttpGet]

    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        Result<List<UsersListResponse>> result = await bus.InvokeAsync<Result<List<UsersListResponse>>>(
            new GetAllUsersRequest(), cancellationToken);
        return result.ToActionResult();
    }
}
