using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Create;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Create;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Warehouses.Create;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Flow.MeasurementPoints;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class CreateMeasuringPointTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    private const string Username = "flowmpcreateuser";
    private const string Password = "K0st@123!";

    [Fact]
    public async Task CreateMeasuringPoint_ShouldSucceed_WithValidParameters()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string ikunkId = await CreateIkunkAsync(client, seed: 1);

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.MeasuringPoints.Base,
            NewRequest(seed: 1, ikunkId),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateMeasuringPoint_ShouldFail_WhenIkunkDoesNotExist()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string nonExistingIkunkId = $"ik_{Guid.NewGuid():N}";

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.MeasuringPoints.Base,
            NewRequest(seed: 2, nonExistingIkunkId),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMeasuringPoint_ShouldFail_WithDuplicateControlPoint()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string ikunkId = await CreateIkunkAsync(client, seed: 3);
        CreateMeasuringPointRequest firstRequest = NewRequest(seed: 3, ikunkId);
        await client.PostAsJsonAsync(Routes.MeasuringPoints.Base, firstRequest, TestContext.Current.CancellationToken);

        CreateMeasuringPointRequest duplicateRequest = firstRequest with { Name = $"MP-CREATE-DIFFERENT-{Guid.NewGuid():N}" };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.MeasuringPoints.Base,
            duplicateRequest,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    private static CreateMeasuringPointRequest NewRequest(int seed, string ikunkId) => new()
    {
        Name = $"MP-CREATE-{seed}-{Guid.NewGuid():N}",
        FullName = $"Measuring point for create tests {seed}",
        ControlPoint = $"BGNCA{Random.Shared.NextInt64(0, 99999999999999):D14}",
        OrderPosition = 1,
        FlowDirection = FlowDirectionType.Incoming,
        Transport = TransportType.Truck,
        ActiveFrom = new DateOnly(2024, 1, 1),
        ActiveTo = new DateOnly(2099, 12, 31),
        IkunkId = ikunkId
    };

    private static async Task<string> CreateWarehouseAsync(HttpClient client, int seed)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Warehouses.Base,
            new CreateWarehouseRequest
            {
                Name = $"WH-MP-CREATE-{seed}-{Guid.NewGuid():N}",
                FullName = $"Warehouse for MeasuringPoint create tests {seed}",
                Description = "Test warehouse",
                OrderPosition = 1,
                ExciseNumber = $"BGNCA{Random.Shared.Next(0, 99999999):D8}",
                ActiveFrom = new DateOnly(2024, 1, 1),
                ActiveTo = new DateOnly(2099, 12, 31)
            },
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        WarehouseResponse warehouse =
            (await response.Content.ReadFromJsonAsync<WarehouseResponse>(TestContext.Current.CancellationToken))!;
        return warehouse.Id;
    }

    private static async Task<string> CreateIkunkAsync(HttpClient client, int seed)
    {
        string warehouseId = await CreateWarehouseAsync(client, seed);

        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Ikunks.Base,
            new CreateIkunkRequest
            {
                Name = $"IK-MP-CREATE-{seed}-{Guid.NewGuid():N}",
                FullName = $"Ikunk for MeasuringPoint create tests {seed}",
                OrderPosition = 1,
                ActiveFrom = new DateOnly(2024, 1, 1),
                ActiveTo = new DateOnly(2099, 12, 31),
                WarehouseId = warehouseId
            },
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        IkunkResponse ikunk =
            (await response.Content.ReadFromJsonAsync<IkunkResponse>(TestContext.Current.CancellationToken))!;
        return ikunk.Id;
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
}
