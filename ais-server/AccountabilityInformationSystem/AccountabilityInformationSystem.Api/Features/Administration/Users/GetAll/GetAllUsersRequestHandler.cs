using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Administration.Users.GetAll;

public sealed class GetAllUsersRequestHandler(
    ApplicationDbContext appDbContext,
    ApplicationIdentityDbContext identityDbContext,
    TimeProvider timeProvider)
{
    public async Task<Result<List<UsersListResponse>>> Handle(
        GetAllUsersRequest _,
        CancellationToken cancellationToken)
    {
        DateTimeOffset now = timeProvider.GetUtcNow();

        // A single query for every identity account and its roles, instead of one role lookup per user.
        Dictionary<string, IdentityAccount> accounts = await identityDbContext.Users
            .AsNoTracking()
            .Select(identityUser => new IdentityAccount(
                identityUser.Id,
                identityUser.EmailConfirmed,
                identityUser.LockoutEnabled && identityUser.LockoutEnd >= now,
                identityDbContext.UserRoles
                    .Where(userRole => userRole.UserId == identityUser.Id)
                    .Join(identityDbContext.Roles, userRole => userRole.RoleId, role => role.Id, (_, role) => role.Name!)
                    .ToList()))
            .ToDictionaryAsync(account => account.Id, cancellationToken);

        List<User> appUsers = await appDbContext.Users.AsNoTracking().ToListAsync(cancellationToken);

        List<UsersListResponse> responseUsers = [.. appUsers.Select(user =>
        {
            IdentityAccount? account = user.IdentityId is null ? null : accounts.GetValueOrDefault(user.IdentityId);
            return user.Adapt<UsersListResponse>() with
            {
                IsConfirmed = account?.EmailConfirmed ?? false,
                IsLocked = account?.IsLocked ?? false,
                Roles = account?.Roles ?? []
            };
        })];

        return Result<List<UsersListResponse>>.Success(responseUsers);
    }

    private sealed record IdentityAccount(string Id, bool EmailConfirmed, bool IsLocked, List<string> Roles);
}
