using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Create;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Update;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Warehouses.Create;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Flow.Ikunks;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class UpdateIkunkTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    private const string Username = "flowikunkupdateuser";
    private const string Password = "K0st@123!";

    [Fact]
    public async Task UpdateIkunk_ShouldSucceed_WithValidParameters()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string warehouseId = await CreateWarehouseAsync(client, seed: 1);
        string ikunkId = await CreateIkunkAsync(client, seed: 1, warehouseId);

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync(
            Routes.Ikunks.Update(ikunkId),
            new { FullName = "Updated full name" },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateIkunk_ShouldFail_WhenIkunkDoesNotExist()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string nonExistingIkunkId = $"ik_{Guid.NewGuid():N}";

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync(
            Routes.Ikunks.Update(nonExistingIkunkId),
            new { FullName = "Updated full name" },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateIkunk_ShouldFail_WithDuplicateName()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string warehouseId = await CreateWarehouseAsync(client, seed: 2);
        string firstName = $"IK-UPDATE-DUP-{Guid.NewGuid():N}";
        await CreateIkunkAsync(client, seed: 2, warehouseId, name: firstName);
        string secondIkunkId = await CreateIkunkAsync(client, seed: 3, warehouseId);

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync(
            Routes.Ikunks.Update(secondIkunkId),
            new { Name = firstName },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task UpdateIkunk_ShouldFail_WhenWarehouseDoesNotExist()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string warehouseId = await CreateWarehouseAsync(client, seed: 4);
        string ikunkId = await CreateIkunkAsync(client, seed: 4, warehouseId);
        string nonExistingWarehouseId = $"wh_{Guid.NewGuid():N}";

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync(
            Routes.Ikunks.Update(ikunkId),
            new { WarehouseId = nonExistingWarehouseId },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task<string> CreateWarehouseAsync(HttpClient client, int seed)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Warehouses.Base,
            new CreateWarehouseRequest
            {
                Name = $"WH-IK-UPDATE-{seed}-{Guid.NewGuid():N}",
                FullName = $"Warehouse for Ikunk update tests {seed}",
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

    private static async Task<string> CreateIkunkAsync(HttpClient client, int seed, string warehouseId, string? name = null)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Ikunks.Base,
            new CreateIkunkRequest
            {
                Name = name ?? $"IK-UPDATE-{seed}-{Guid.NewGuid():N}",
                FullName = $"Ikunk for update tests {seed}",
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
