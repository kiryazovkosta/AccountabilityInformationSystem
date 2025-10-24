using AccountabilityInformationSystem.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Extensions;

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
            Console.WriteLine("Successfully applied migration to ApplicationDbContext!");

            await identityDbContext.Database.MigrateAsync(cancellationToken);
            Console.WriteLine("Successfully applied migration to ApplicationIdentityDbContext!");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }
}
