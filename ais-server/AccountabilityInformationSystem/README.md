# AccountabilityInformationSystem

All commands below are run from this directory (`ais-server/AccountabilityInformationSystem`).

## Build

```bash
# Build the solution
dotnet build AccountabilityInformationSystem.sln

# Full rebuild (ignores previous build outputs)
dotnet build AccountabilityInformationSystem.sln --no-incremental

# Clean everything first, then rebuild
dotnet clean AccountabilityInformationSystem.sln && dotnet build AccountabilityInformationSystem.sln
```

## Test

```bash
# Build and run all tests
dotnet test AccountabilityInformationSystem.sln

# Run all tests without rebuilding
dotnet test AccountabilityInformationSystem.sln --no-build

# Run a single test project
dotnet test AccountabilityInformationSystem.UnitTests
dotnet test AccountabilityInformationSystem.IntegrationTests

# Run only matching tests, e.g. the auth ones
dotnet test AccountabilityInformationSystem.sln --filter "FullyQualifiedName~Identity.Auth"
```

## Run

```bash
# Start the whole app (API, client, SQL Server, MailPit, Azurite) with Aspire
dotnet run --project AccountabilityInformationSystem.Aspire.AppHost
```

The console prints the Aspire dashboard login URL. In Development, emails are not delivered to real inboxes: they are caught by MailPit, which you can open from the **mailpit** resource in the dashboard.
