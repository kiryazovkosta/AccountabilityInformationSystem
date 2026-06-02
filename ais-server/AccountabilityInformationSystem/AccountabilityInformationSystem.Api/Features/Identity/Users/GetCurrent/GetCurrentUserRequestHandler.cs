using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.AspNetCore.Identity;

namespace AccountabilityInformationSystem.Api.Features.Identity.Users.GetCurrent;

public sealed class GetCurrentUserRequestHandler(
    UserContext userContext,
    UserManager<IdentityUser> userManager)
{
    public async Task<Result<UserResponse>> Handle(GetCurrentUserRequest _, CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result<UserResponse>.Failure(
                new Error("Unauthorized", "Unauthorized"),
                ResultFailureType.Unauthorized);
        }

        IList<string> roles = [];
        if (!string.IsNullOrEmpty(user.IdentityId))
        {
            roles = await userManager.GetRolesAsync(new IdentityUser { Id = user.IdentityId });
        }

        return Result<UserResponse>.Success(user.ToResponse(roles));
    }
}
