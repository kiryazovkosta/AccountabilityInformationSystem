using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
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

        List<UsersListResponse> responseUsers = [.. await Task.WhenAll(
            appUsers.Select(async x =>
            {
                IdentityUser? identityUser = identityUsers.FirstOrDefault(iu => iu.Id == x.IdentityId);
                return new UsersListResponse
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    Image = x.Image,
                    Enable2Fa = x.Enable2Fa,
                    IdentityId = x.IdentityId,
                    IsConfirmed = identityUser?.EmailConfirmed ?? false,
                    IsLocked = identityUser is not null && identityUser.LockoutEnabled && identityUser.LockoutEnd >= DateTimeOffset.UtcNow
                };
            })
        )];

        return Result<List<UsersListResponse>>.Success(responseUsers);
    }
}

public class UsersListResponse
{
    public string Id { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string LastName { get; init; }
    public string? Image { get; init; }
    public bool? Enable2Fa { get; init; }
    public string? IdentityId { get; init; }
    public bool IsConfirmed {  get; init; }
    public bool IsLocked { get; init; }
}
