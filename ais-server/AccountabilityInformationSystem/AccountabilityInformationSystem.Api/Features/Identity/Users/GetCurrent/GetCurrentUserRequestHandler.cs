using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;

namespace AccountabilityInformationSystem.Api.Features.Identity.Users.GetCurrent;

public sealed class GetCurrentUserRequestHandler(UserContext userContext)
{
    public async Task<Result<UserResponse>> Handle(GetCurrentUserRequest _, CancellationToken cancellationToken)
    {
        var user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result<UserResponse>.Failure(
                new Error("Unauthorized", "Unauthorized"),
                ResultFailureType.Unauthorized);
        }

        return Result<UserResponse>.Success(user.ToResponse());
    }
}
