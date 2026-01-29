using Aspire.Hosting;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("ais-mssql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("ais-mssqldata")
    .WithHostPort(11433);

var database = sqlServer.AddDatabase("AccountabilityInformationSystemDb");

builder.AddProject<Projects.AccountabilityInformationSystem_Api>("ais-api")
    .WithReference(database)
    .WaitFor(sqlServer)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

await builder.Build().RunAsync();
