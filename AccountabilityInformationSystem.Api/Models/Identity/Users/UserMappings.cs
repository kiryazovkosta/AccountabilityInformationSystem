using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Entities.Identity;
using AccountabilityInformationSystem.Api.Models.Identity.Auth;

namespace AccountabilityInformationSystem.Api.Models.Identity.Users;

public static class UserMappings
{
    public static User ToEntity(this RegisterUserRequest request)
    {
        return new User
        {
            Id = $"u_{Guid.CreateVersion7()}",
            Email = request.Email,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Image = request.Image,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = EntitiesConstants.DefaultSystemUser
        };
    }
}
