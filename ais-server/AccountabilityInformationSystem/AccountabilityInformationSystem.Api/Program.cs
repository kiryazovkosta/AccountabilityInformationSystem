using AccountabilityInformationSystem.Api.Settings;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Aspire.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Wolverine;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults()
    .AddApiServices()
    .AddValidators()
    .AddErrorHandling()
    .AddDatabase()
    .AddObservability()
    .AddApplicationServices()
    .AddFeatureModules()
    .AddAuthenticationServices()
    .AddAzureBlobFilesStorage()
    .AddCorsPolicy()
    .AddMapster()
    .AddWolverine();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();
app.MapOpenApi();
app.MapScalarApiReference();
if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationAsync();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseCors(CorsOptions.PolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCaching();
app.UseAntiforgery();
app.MapControllers();
await app.RunAsync();
