using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities;
using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Middleware;
using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;
using AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;
using AccountabilityInformationSystem.Api.Models.Warehouses;
using AccountabilityInformationSystem.Api.Services.DataShaping;
using AccountabilityInformationSystem.Api.Services.Linking;
using AccountabilityInformationSystem.Api.Services.Sorting;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Newtonsoft.Json.Serialization;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AccountabilityInformationSystem.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddControllers(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlSerializerFormatters();

        builder.Services.AddOpenApi();

        return builder;
    }

    public static WebApplicationBuilder AddValidators(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
        return builder;
    }

    public static WebApplicationBuilder AddErrorHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = (context) =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options
                .UseSqlServer(
                    builder.Configuration.GetConnectionString(DatabasesConstants.ApplicationDatabase),
                    sqlOptions => sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, ShemasConstants.Application));
        });

        return builder;
    }

    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
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

        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<SortMappingProvider>();
        builder.Services
            .AddSingleton<ISortMappingDefinition, SortMappingDefinition<MeasurementPointResponse, MeasurementPoint>>(_ =>
                MeasurementPointMappings.SortMapping);
        builder.Services
            .AddSingleton<ISortMappingDefinition, SortMappingDefinition<WarehouseResponse, Warehouse>>(_ =>
                WarehouseMappings.SortMapping);
        builder.Services
            .AddSingleton<ISortMappingDefinition, SortMappingDefinition<IkunkResponse, Ikunk>>(_ =>
                IkunkMappings.SortMapping);

        builder.Services.AddTransient<DataShapingService>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<LinkService>();

        return builder;
    }
}
