using System.Diagnostics;
using System.Runtime.InteropServices;
using Aspire.Hosting;
using Aspire.Hosting.JavaScript;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<SqlServerServerResource> sqlServer = builder.AddSqlServer("ais-mssql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("ais-mssqldata")
    .WithHostPort(11433);

IResourceBuilder<SqlServerDatabaseResource> database = sqlServer.AddDatabase("AccountabilityInformationSystemDb");

IResourceBuilder<MailPitContainerResource> mailpit = builder.AddMailPit("mailpit")
    .WithDataVolume("mailpit-data");

IResourceBuilder<ProjectResource> backend = builder.AddProject<Projects.AccountabilityInformationSystem_Api>("ais-api")
    .WithReference(mailpit)
    .WaitFor(mailpit)
    .WithReference(database)
    .WaitFor(sqlServer)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

builder.Eventing.Subscribe<BeforeStartEvent>(async (ev, ct) =>
{
    string certPath = Path.GetFullPath(
        Path.Combine(builder.AppHostDirectory, "../../../ais-client/localhost.pem"));

    if (!File.Exists(certPath))
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"dev-certs https --export-path \"{certPath}\" --format Pem --no-password",
            UseShellExecute = false
        };
        using Process process = Process.Start(psi)!;
        await process.WaitForExitAsync(ct);
    }
});

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    builder.AddExecutable(
    "ais-client", "npm.cmd", workingDirectory: "../../../ais-client",
    "run", "start", "--", "--port", "4201",
    "--ssl", "--ssl-cert", "localhost.pem", "--ssl-key", "localhost.key")
    .WithHttpsEndpoint(port: 4200, targetPort: 4201, name: "https")
    .WithEnvironment("BROWSER", "none")
    .WaitFor(backend);
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    builder.AddExecutable(
    "ais-client", "npm", workingDirectory: "../../../ais-client",
    "run", "start", "--", "--port", "4201",
    "--ssl", "--ssl-cert", "localhost.pem", "--ssl-key", "localhost.key")
    .WithHttpsEndpoint(port: 4200, targetPort: 4201, name: "https")
    .WithEnvironment("BROWSER", "none")
    .WaitFor(backend);
}

await builder.Build().RunAsync();
