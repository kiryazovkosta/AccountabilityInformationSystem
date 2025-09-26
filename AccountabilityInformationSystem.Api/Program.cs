using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options
        .UseSqlServer(
            builder.Configuration.GetConnectionString(DatabasesConstants.ApplicationDatabase),
            sqlOptions => sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, ShemasConstants.Application));
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
    .WithTracing(tracing => tracing
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddSqlClientInstrumentation()
        )
    .WithMetrics(metrics => metrics
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation())
    .UseOtlpExporter();

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeScopes = true;
    options.IncludeFormattedMessage = true;
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    await app.ApplyMigrationAsync();
}

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
