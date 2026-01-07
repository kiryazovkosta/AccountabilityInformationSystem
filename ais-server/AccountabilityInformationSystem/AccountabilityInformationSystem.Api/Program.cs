using AccountabilityInformationSystem.Api.Extensions;
using AccountabilityInformationSystem.Api.Settings;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder
    .AddApiServices()
    .AddValidators()
    .AddErrorHandling()
    .AddDatabase()
    .AddObservability()
    .AddApplicationServices()
    .AddAuthenticationServices()
    .AddCorsPolicy();

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    await app.ApplyMigrationAsync();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseCors(CorsOptions.PolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
