using AccountabilityInformationSystem.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
            Console.WriteLine("Successfully applied migration to ApplicationDbContext!");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }
}
