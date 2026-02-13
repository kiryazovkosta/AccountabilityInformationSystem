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

builder.AddExecutable(
    "ais-client", "npm.cmd", workingDirectory: "../../../ais-client", "run", "start")
    .WithEnvironment("BROWSER", "none")
    .WaitFor(backend);

await builder.Build().RunAsync();
