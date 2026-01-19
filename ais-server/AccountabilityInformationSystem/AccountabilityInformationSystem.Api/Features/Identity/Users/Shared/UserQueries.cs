using System.Linq.Expressions;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;

namespace AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;

public class UserQueries
{
    public static Expression<Func<User, UserResponse>> ProjectToResponse()
    {
        return user => new UserResponse
        {
            Id = user.Id,
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
