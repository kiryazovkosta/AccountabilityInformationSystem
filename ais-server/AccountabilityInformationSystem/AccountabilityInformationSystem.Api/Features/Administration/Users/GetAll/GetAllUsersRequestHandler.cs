using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Administration.Users.GetAll;

public sealed class GetAllUsersRequestHandler(
    ApplicationDbContext appDbContext,
    UserManager<IdentityUser> userManager)
{
    public async Task<Result<List<UsersListResponse>>> Handle(
        GetAllUsersRequest _,
        CancellationToken cancellationToken)
    {
        List<IdentityUser> identityUsers = await userManager.Users.ToListAsync(cancellationToken);

        List<User> appUsers = await appDbContext.Users.ToListAsync(cancellationToken);

        var responseUsers = new List<UsersListResponse>(appUsers.Count);
        foreach (User x in appUsers)
        {
            IdentityUser? identityUser = identityUsers.FirstOrDefault(iu => iu.Id == x.IdentityId);
            IList<string> roles = identityUser is not null
                ? await userManager.GetRolesAsync(identityUser)
                : [];
            responseUsers.Add(x.Adapt<UsersListResponse>() with
            {
                IsConfirmed = identityUser?.EmailConfirmed ?? false,
                IsLocked = identityUser is not null && identityUser.LockoutEnabled && identityUser.LockoutEnd >= DateTimeOffset.UtcNow,
                Roles = roles
            });
        }

        return Result<List<UsersListResponse>>.Success(responseUsers);
    }
}
