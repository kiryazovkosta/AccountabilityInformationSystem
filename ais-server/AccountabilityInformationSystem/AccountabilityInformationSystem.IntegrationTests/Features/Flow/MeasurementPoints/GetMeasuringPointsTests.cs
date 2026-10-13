using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Create;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Create;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Features.Warehouses.Create;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Flow.MeasurementPoints;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class GetMeasuringPointsTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    private const string Username = "flowmpgetuser";
    private const string Password = "K0st@123!";

    [Theory]
    [InlineData(CustomMediaTypeNames.Application.HateoasJson)]
    [InlineData(CustomMediaTypeNames.Application.HateoasJsonV2)]
    public async Task GetMeasuringPoints_ShouldIncludeCollectionAndItemLinks_WhenHateoasIsRequested(string accept)
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string ikunkId = await CreateIkunkAsync(client);
        await CreateMeasuringPointAsync(client, ikunkId);
        await CreateMeasuringPointAsync(client, ikunkId);

        using HttpRequestMessage request = new(HttpMethod.Get, $"{Routes.MeasuringPoints.Base}?ikunkId={ikunkId}&pageSize=1");
        request.Headers.Accept.ParseAdd(accept);

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonElement page = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        Assert.Equal(["self", "create", "next-page"], Rels(page));
        JsonElement item = Assert.Single(page.GetProperty("items").EnumerateArray());
        Assert.Equal(["self", "update", "deactivate"], Rels(item));
    }

    private static List<string> Rels(JsonElement element) =>
        [.. element.GetProperty("links").EnumerateArray().Select(link => link.GetProperty("rel").GetString()!)];

    private static async Task CreateMeasuringPointAsync(HttpClient client, string ikunkId)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.MeasuringPoints.Base,
            new CreateMeasuringPointRequest
            {
                Name = $"MP-GET-{Guid.NewGuid():N}",
                FullName = "Measuring point for get tests",
                ControlPoint = $"BGNCA{Random.Shared.NextInt64(0, 99999999999999):D14}",
                OrderPosition = 1,
                FlowDirection = FlowDirectionType.Incoming,
                Transport = TransportType.Truck,
                ActiveFrom = new DateOnly(2024, 1, 1),
                ActiveTo = new DateOnly(2099, 12, 31),
                IkunkId = ikunkId
            },
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private static async Task<string> CreateIkunkAsync(HttpClient client)
    {
        HttpResponseMessage warehouseResponse = await client.PostAsJsonAsync(
            Routes.Warehouses.Base,
            new CreateWarehouseRequest
            {
                Name = $"WH-MP-GET-{Guid.NewGuid():N}",
                FullName = "Warehouse for MeasuringPoint get tests",
                OrderPosition = 1,
                ExciseNumber = $"BGNCA{Random.Shared.Next(0, 99999999):D8}",
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
                Name = $"IK-MP-GET-{Guid.NewGuid():N}",
                FullName = "Ikunk for MeasuringPoint get tests",
                OrderPosition = 1,
                ActiveFrom = new DateOnly(2024, 1, 1),
                ActiveTo = new DateOnly(2099, 12, 31),
                WarehouseId = warehouse.Id
            },
            TestContext.Current.CancellationToken);
        ikunkResponse.EnsureSuccessStatusCode();
        IkunkResponse ikunk =
            (await ikunkResponse.Content.ReadFromJsonAsync<IkunkResponse>(TestContext.Current.CancellationToken))!;
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
