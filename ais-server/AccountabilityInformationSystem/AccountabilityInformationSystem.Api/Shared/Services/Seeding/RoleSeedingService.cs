using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace AccountabilityInformationSystem.Api.Shared.Services.Seeding;

public sealed class RoleSeedingService(IServiceProvider serviceProvider) : IHostedService
{
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
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
