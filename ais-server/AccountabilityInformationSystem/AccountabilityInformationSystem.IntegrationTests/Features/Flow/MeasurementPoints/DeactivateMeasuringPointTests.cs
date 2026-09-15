using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Create;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Create;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Warehouses.Create;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Flow.MeasurementPoints;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class DeactivateMeasuringPointTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    private const string Username = "flowmpuser";
    private const string Password = "K0st@123!";

    [Fact]
    public async Task DeactivateMeasuringPoint_ShouldSucceed_WithValidActiveTo()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string measuringPointId = await CreateMeasuringPointAsync(client, seed: 1);

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync(
            Routes.MeasuringPoints.Deactivate(measuringPointId),
            new { ActiveTo = DateOnly.FromDateTime(DateTime.Today.AddDays(5)) },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeactivateMeasuringPoint_ShouldFail_WithPastActiveTo()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string measuringPointId = await CreateMeasuringPointAsync(client, seed: 2);

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync(
            Routes.MeasuringPoints.Deactivate(measuringPointId),
            new { ActiveTo = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)) },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeactivateMeasuringPoint_ShouldFail_WhenMeasuringPointDoesNotExist()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync(
            Routes.MeasuringPoints.Deactivate("mp_does-not-exist"),
            new { ActiveTo = DateOnly.FromDateTime(DateTime.Today.AddDays(5)) },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeactivateMeasuringPoint_ShouldUseRouteId_WhenBodyContainsAnIdField()
    {
        // Arrange — regression test: `Id` on DeactivateMeasuringPointRequest is `internal`, so it can never be
        // bound from the request body. The route segment must remain the sole source of truth for which
        // measuring point is affected, even if a client sends an "id" field in the JSON body.
        HttpClient client = await CreateFlowUserClientAsync();
        string measuringPointId = await CreateMeasuringPointAsync(client, seed: 3);

        using StringContent content = new(
            $$"""{"id":"mp_some-other-nonexistent-id","activeTo":"{{DateOnly.FromDateTime(DateTime.Today.AddDays(5)):yyyy-MM-dd}}"}""",
            Encoding.UTF8,
            "application/json");

        // Act
        HttpResponseMessage response = await client.PutAsync(
            Routes.MeasuringPoints.Deactivate(measuringPointId),
            content,
            TestContext.Current.CancellationToken);

        // Assert — if the server had honored a body-supplied id, this would be 404 (that id doesn't exist).
        // Success here proves the route id was used instead.
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task<HttpClient> CreateFlowUserClientAsync()
    {
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync(Username, Password);

        using IServiceScope scope = factory.Services.CreateScope();
        UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        IdentityUser identityUser = (await userManager.FindByNameAsync(Username))!;
        if (!await userManager.IsInRoleAsync(identityUser, Role.FlowUser))
        {
            await userManager.AddToRoleAsync(identityUser, Role.FlowUser);
        }

        HttpResponseMessage loginResponse = await client.PostAsJsonAsync(
            Routes.Auth.Login,
            new LoginUserRequest { Username = Username, Password = Password },
            TestContext.Current.CancellationToken);
        loginResponse.EnsureSuccessStatusCode();

        LoginUserResponse loginResult =
            (await loginResponse.Content.ReadFromJsonAsync<LoginUserResponse>(TestContext.Current.CancellationToken))!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

        return client;
    }

    private static async Task<string> CreateMeasuringPointAsync(HttpClient client, int seed)
    {
        HttpResponseMessage warehouseResponse = await client.PostAsJsonAsync(
            Routes.Warehouses.Base,
            new CreateWarehouseRequest
            {
                Name = $"WH-MP-{seed}",
                FullName = $"Warehouse for MeasuringPoint tests {seed}",
                Description = "Test warehouse",
                OrderPosition = 1,
                ExciseNumber = $"BGNCA{seed:D8}",
                ActiveFrom = new DateOnly(2024, 1, 1),
                ActiveTo = new DateOnly(2099, 12, 31)
            },
            TestContext.Current.CancellationToken);
        warehouseResponse.EnsureSuccessStatusCode();
        WarehouseResponse warehouse =
            (await warehouseResponse.Content.ReadFromJsonAsync<WarehouseResponse>(TestContext.Current.CancellationToken))!;

        HttpResponseMessage ikunkResponse = await client.PostAsJsonAsync(
            Routes.Ikunks.Base,
            new CreateIkunkRequest
            {
                Name = $"IK-{seed}",
                FullName = $"Ikunk {seed}",
                OrderPosition = 1,
                ActiveFrom = new DateOnly(2024, 1, 1),
                ActiveTo = new DateOnly(2099, 12, 31),
                WarehouseId = warehouse.Id
            },
            TestContext.Current.CancellationToken);
        ikunkResponse.EnsureSuccessStatusCode();
        IkunkResponse ikunk =
            (await ikunkResponse.Content.ReadFromJsonAsync<IkunkResponse>(TestContext.Current.CancellationToken))!;

        HttpResponseMessage measuringPointResponse = await client.PostAsJsonAsync(
            Routes.MeasuringPoints.Base,
            new CreateMeasuringPointRequest
            {
                Name = $"MP-{seed}",
                FullName = $"Measuring Point {seed}",
                ControlPoint = $"BGNCA{seed:D14}",
                OrderPosition = 1,
                FlowDirection = FlowDirectionType.Incoming,
                Transport = TransportType.Truck,
                ActiveFrom = new DateOnly(2024, 1, 1),
                ActiveTo = new DateOnly(2099, 12, 31),
                IkunkId = ikunk.Id
            },
            TestContext.Current.CancellationToken);
        measuringPointResponse.EnsureSuccessStatusCode();
        MeasurementPointResponse measurementPoint =
            (await measuringPointResponse.Content.ReadFromJsonAsync<MeasurementPointResponse>(TestContext.Current.CancellationToken))!;

        return measurementPoint.Id;
    }
}
