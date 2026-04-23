using System.Net.Http.Headers;
using System.Net.Http.Json;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure.Stubs;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;

namespace AccountabilityInformationSystem.IntegrationTests.Infrastructure;

public class AisAuthWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
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
        builder.UseEnvironment("Development");

        builder.UseSetting(
            "ConnectionStrings:AccountabilityInformationSystemDb",
            _mssqlContainer.GetConnectionString());

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IEmailSender>();
            services.AddScoped<IEmailSender, NullEmailSender>();

            services.PostConfigure<AntiforgeryOptions>(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        });
    }

    public async Task<HttpClient> CreateAuthenticatedClientWithout2FaAsync(
        string username = "warehouseuser",
        string password = "K0st@123!")
    {
        HttpClient client = CreateClient();

        using IServiceScope scope = Services.CreateScope();
        using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        bool userExists = await dbContext.Users.AnyAsync(x => x.Username == username);

        if (!userExists)
        {
            HttpResponseMessage registerResponse = await client.PostAsJsonAsync(Routes.Auth.Register,
                new RegisterUserRequest
                {
                    Username = username,
                    Email = $"{username}@example.com",
                    FirstName = "Warehouse",
                    LastName = "User",
                    Password = password,
                    ConfirmPassword = password,
                    Enable2Fa = false
                });
            registerResponse.EnsureSuccessStatusCode();
        }

        HttpResponseMessage loginResponse = await client.PostAsJsonAsync(Routes.Auth.Login,
            new LoginUserRequest { Username = username, Password = password });
        loginResponse.EnsureSuccessStatusCode();

        LoginUserResponse loginResult = (await loginResponse.Content.ReadFromJsonAsync<LoginUserResponse>())!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

        return client;
    }
}
