using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;

namespace AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;

public static class UserMappings
{
    public static User ToEntity(this RegisterUserRequest request)
    {
        return new User
        {
            Id = $"u_{Guid.CreateVersion7()}",
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Image = request.Image, 
            Enable2Fa = request.Enable2Fa,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = EntitiesConstants.DefaultSystemUser
        };
    }

    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName =
                user.MiddleName != null ?
                    $"{user.FirstName} {user.MiddleName} {user.LastName}"
                    : $"{user.FirstName} {user.LastName}",
            Image = user.Image,
            CreatedAt = user.CreatedAt,
            ModifiedAt = user.ModifiedAt
        };
    }
}
