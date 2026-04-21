using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure.Stubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;

namespace AccountabilityInformationSystem.IntegrationTests.Infrastructure;

public sealed class AisWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _mssqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public new async Task DisposeAsync()
    {
        await _mssqlContainer.StopAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await _mssqlContainer.StartAsync();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        string connectionString = _mssqlContainer.GetConnectionString();

        using ApplicationDbContext appDb = new(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString, sql =>
                    sql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, SchemasConstants.Application))
                .Options);
        appDb.Database.Migrate();

        using ApplicationIdentityDbContext identityDb = new(
            new DbContextOptionsBuilder<ApplicationIdentityDbContext>()
                .UseSqlServer(connectionString, sql =>
                    sql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, SchemasConstants.Identity))
                .Options);
        identityDb.Database.Migrate();

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting(
            "ConnectionStrings:AccountabilityInformationSystemDb",
            _mssqlContainer.GetConnectionString());

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IEmailSender>();
            services.AddScoped<IEmailSender, NullEmailSender>();
        });
    }
}
