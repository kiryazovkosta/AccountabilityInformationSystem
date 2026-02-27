using System.Runtime.InteropServices;
using Aspire.Hosting;
using Aspire.Hosting.JavaScript;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<SqlServerServerResource> sqlServer = builder.AddSqlServer("ais-mssql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("ais-mssqldata")
    .WithHostPort(11433);

IResourceBuilder<SqlServerDatabaseResource> database = sqlServer.AddDatabase("AccountabilityInformationSystemDb");

IResourceBuilder<ProjectResource> backend = builder.AddProject<Projects.AccountabilityInformationSystem_Api>("ais-api")
    .WithReference(database)
    .WaitFor(sqlServer)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    builder.AddExecutable(
    "ais-client", "npm.cmd", workingDirectory: "../../../ais-client", "run", "start", "--", "--port", "4201")
    .WithHttpEndpoint(port: 4200, targetPort: 4201, name: "http")
    .WithEnvironment("BROWSER", "none")
    .WaitFor(backend);
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    builder.AddExecutable(
    "ais-client", "npm", workingDirectory: "../../../ais-client", "run", "start", "--", "--port", "4201")
    .WithHttpEndpoint(port: 4200, targetPort: 4201, name: "http")
    .WithEnvironment("BROWSER", "none")
    .WaitFor(backend);
}

await builder.Build().RunAsync();