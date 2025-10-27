using System.Security.Claims;
using System.Text;
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
using AccountabilityInformationSystem.Api.Services.Tokenizing;
using AccountabilityInformationSystem.Api.Services.UserContexting;
using AccountabilityInformationSystem.Api.Settings;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AccountabilityInformationSystem.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {

        builder.Services
            .AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

        builder.Services.Configure<MvcOptions>(options =>
        {
            NewtonsoftJsonOutputFormatter formatter = options.OutputFormatters
                .OfType<NewtonsoftJsonOutputFormatter>()
                .First();
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.HateoasJson);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.JsonV1);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.JsonV2);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.HateoasJsonV1);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.HateoasJsonV2);

        });

        builder.Services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionSelector = new DefaultApiVersionSelector(options);
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new MediaTypeApiVersionReader(),
                    new MediaTypeApiVersionReaderBuilder()
                        .Template("application/vnd.ais.hateoas.{version}+json")
                        .Build());
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
        //builder.Services.AddOpenApi();

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
                    sqlOptions => sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, SchemasConstants.Application));
        });

        builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
        {
            options
                .UseSqlServer(
                    builder.Configuration.GetConnectionString(DatabasesConstants.ApplicationDatabase),
                    sqlOptions => sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, SchemasConstants.Identity));
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

        builder.Services.AddTransient<TokenProvider>();

        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<UserContext>();

        return builder;
    }

    public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.Configure<JwtAuthOptions>(builder.Configuration.GetSection("Jwt"));

        JwtAuthOptions? jwtAuthOptions = builder.Configuration.GetSection("Jwt").Get<JwtAuthOptions>() ?? throw new Exception("Invalid or non exists jwt section!");

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtAuthOptions.Issuer,
                    ValidAudience = jwtAuthOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.Key))
                };
            });

        builder.Services.AddAuthorization();

        return builder;
    }
}
