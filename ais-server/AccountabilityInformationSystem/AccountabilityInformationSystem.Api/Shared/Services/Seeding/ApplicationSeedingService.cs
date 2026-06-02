using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Settings;
using AccountabilityInformationSystem.Api.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace AccountabilityInformationSystem.Api.Shared.Services.Seeding;

public sealed class ApplicationSeedingService(
    IServiceProvider serviceProvider,
    IOptions<SeedingOptions> options) : IHostedService
{
    private readonly SeedingOptions _options = options.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = [Role.Admin, Role.FlowUser, Role.Member];
        foreach (string role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (string.IsNullOrEmpty(_options.AdminEmail) || string.IsNullOrEmpty(_options.AdminPassword))
        {
            return;
        }

        UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        ApplicationIdentityDbContext identityDbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
        ApplicationDbContext appDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await identityDbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            if (await userManager.FindByEmailAsync(_options.AdminEmail) is not null)
            {
                return;
            }

            await using IDbContextTransaction tx = await identityDbContext.Database.BeginTransactionAsync(cancellationToken);
            appDb.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
            await appDb.Database.UseTransactionAsync(tx.GetDbTransaction(), cancellationToken);

            IdentityUser identityUser = new()
            {
                UserName = _options.AdminUsername,
                Email = _options.AdminEmail,
                EmailConfirmed = true,
            };

            IdentityResult result = await userManager.CreateAsync(identityUser, _options.AdminPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Admin seeding failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await userManager.AddToRoleAsync(identityUser, Role.Admin);

            appDb.Users.Add(new User
            {
                Id = $"u_{Guid.CreateVersion7()}",
                IdentityId = identityUser.Id,
                Username = _options.AdminUsername,
                Email = _options.AdminEmail,
                FirstName = _options.AdminFirstName,
                LastName = _options.AdminLastName,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = EntitiesConstants.DefaultSystemUser,
            });
            await appDb.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);
        });
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
