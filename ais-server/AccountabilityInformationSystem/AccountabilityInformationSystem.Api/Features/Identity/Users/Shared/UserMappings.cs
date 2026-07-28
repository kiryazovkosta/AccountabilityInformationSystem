using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using Mapster;

namespace AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;

public static class UserMappings
{
    public static UserResponse ToResponse(this User user, IList<string>? roles = null)
        => user.Adapt<UserResponse>() with { Roles = roles?.AsReadOnly() ?? [] };
}
