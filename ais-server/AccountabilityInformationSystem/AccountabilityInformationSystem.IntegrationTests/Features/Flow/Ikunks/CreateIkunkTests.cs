using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Create;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Warehouses.Create;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Flow.Ikunks;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class CreateIkunkTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    private const string Username = "flowikunkcreateuser";
    private const string Password = "K0st@123!";

    [Fact]
    public async Task CreateIkunk_ShouldSucceed_WithValidParameters()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string warehouseId = await CreateWarehouseAsync(client, seed: 1);

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Ikunks.Base,
            NewRequest(seed: 1, warehouseId),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateIkunk_ShouldFail_WhenWarehouseDoesNotExist()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string nonExistingWarehouseId = $"wh_{Guid.NewGuid():N}";

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Ikunks.Base,
            NewRequest(seed: 2, nonExistingWarehouseId),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateIkunk_ShouldFail_WithDuplicateName()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string warehouseId = await CreateWarehouseAsync(client, seed: 3);
        CreateIkunkRequest request = NewRequest(seed: 3, warehouseId);
        await client.PostAsJsonAsync(Routes.Ikunks.Base, request, TestContext.Current.CancellationToken);

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Ikunks.Base,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    private static CreateIkunkRequest NewRequest(int seed, string warehouseId) => new()
    {
        Name = $"IK-CREATE-{seed}-{Guid.NewGuid():N}",
        FullName = $"Ikunk for create tests {seed}",
        OrderPosition = 1,
        ActiveFrom = new DateOnly(2024, 1, 1),
        ActiveTo = new DateOnly(2099, 12, 31),
        WarehouseId = warehouseId
    };

    private static async Task<string> CreateWarehouseAsync(HttpClient client, int seed)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Warehouses.Base,
            new CreateWarehouseRequest
            {
                Name = $"WH-IK-CREATE-{seed}-{Guid.NewGuid():N}",
                FullName = $"Warehouse for Ikunk create tests {seed}",
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
