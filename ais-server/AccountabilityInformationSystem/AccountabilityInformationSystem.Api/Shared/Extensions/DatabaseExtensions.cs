using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Shared.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using ApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await using ApplicationIdentityDbContext identityDbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
        try
        {
            await applicationDbContext.Database.MigrateAsync(cancellationToken);

            await identityDbContext.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }
}
