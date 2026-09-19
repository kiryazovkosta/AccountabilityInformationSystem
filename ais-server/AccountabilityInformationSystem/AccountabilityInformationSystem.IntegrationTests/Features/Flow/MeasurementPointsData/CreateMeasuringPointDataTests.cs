using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AccountabilityInformationSystem.Api.Domain.Entities;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Create;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Create;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Create;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.ProductTypes.Create;
using AccountabilityInformationSystem.Api.Features.ProductTypes.Shared;
using AccountabilityInformationSystem.Api.Features.Warehouses.Create;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Flow.MeasurementPointsData;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class CreateMeasuringPointDataTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    private const string Username = "flowmpdatacreateuser";
    private const string Password = "K0st@123!";

    [Fact]
    public async Task CreateMeasuringPointData_ShouldSucceed_WithValidParameters()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string measurementPointId = await CreateMeasurementPointAsync(client, seed: 1);
        string productId = await SeedProductAsync(client, seed: 1);

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.MeasurementPointsData.Base,
            NewRequest(measurementPointId, productId, number: 1),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        MeasurementPointDataResponse created =
            (await response.Content.ReadFromJsonAsync<MeasurementPointDataResponse>(TestContext.Current.CancellationToken))!;
        Assert.NotEmpty(created.Links);
    }

    [Fact]
    public async Task CreateMeasuringPointData_ShouldFail_WhenMeasurementPointDoesNotExist()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string productId = await SeedProductAsync(client, seed: 2);
        string nonExistingMeasurementPointId = $"mp_{Guid.NewGuid():N}";

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.MeasurementPointsData.Base,
            NewRequest(nonExistingMeasurementPointId, productId, number: 2),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMeasuringPointData_ShouldFail_WhenProductDoesNotExist()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string measurementPointId = await CreateMeasurementPointAsync(client, seed: 3);
        string nonExistingProductId = $"pr_{Guid.NewGuid():N}";

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.MeasurementPointsData.Base,
            NewRequest(measurementPointId, nonExistingProductId, number: 3),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMeasuringPointData_ShouldFail_WithDuplicateParameters()
    {
        // Arrange
        HttpClient client = await CreateFlowUserClientAsync();
        string measurementPointId = await CreateMeasurementPointAsync(client, seed: 4);
        string productId = await SeedProductAsync(client, seed: 4);
        object request = NewRequest(measurementPointId, productId, number: 4);
        await client.PostAsJsonAsync(Routes.MeasurementPointsData.Base, request, TestContext.Current.CancellationToken);

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.MeasurementPointsData.Base,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    private static object NewRequest(string measurementPointId, string productId, int number) => new CreateMeasuringPointDataRequest
    {
        MeasurementPointId = measurementPointId,
        Number = number,
        BeginTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        EndTime = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc),
        FlowDirectionType = FlowDirectionType.Incoming,
        ProductId = productId
    };

    private static async Task<string> CreateWarehouseAsync(HttpClient client, int seed)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Warehouses.Base,
            new CreateWarehouseRequest
            {
                Name = $"WH-MPD-{seed}-{Guid.NewGuid():N}",
                FullName = $"Warehouse for MeasuringPointData tests {seed}",
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
                Name = $"IK-MPD-{seed}-{Guid.NewGuid():N}",
                FullName = $"Ikunk for MeasuringPointData tests {seed}",
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

    private static async Task<string> CreateMeasurementPointAsync(HttpClient client, int seed)
    {
        string ikunkId = await CreateIkunkAsync(client, seed);

        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.MeasuringPoints.Base,
            new CreateMeasuringPointRequest
            {
                Name = $"MP-MPD-{seed}-{Guid.NewGuid():N}",
                FullName = $"Measuring point for MeasuringPointData tests {seed}",
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

        MeasurementPointResponse measurementPoint =
            (await response.Content.ReadFromJsonAsync<MeasurementPointResponse>(TestContext.Current.CancellationToken))!;
        return measurementPoint.Id;
    }

    private async Task<string> SeedProductAsync(HttpClient client, int seed)
    {
        HttpResponseMessage productTypeResponse = await client.PostAsJsonAsync(
            Routes.ProductTypes.Base,
            new CreateProductTypeRequest
            {
                Name = $"PT-MPD-{seed}-{Guid.NewGuid():N}",
                FullName = $"Product Type for MeasuringPointData tests {seed}"
            },
            TestContext.Current.CancellationToken);
        productTypeResponse.EnsureSuccessStatusCode();
        ProductTypeResponse productType =
            (await productTypeResponse.Content.ReadFromJsonAsync<ProductTypeResponse>(TestContext.Current.CancellationToken))!;

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Product product = new()
        {
            Id = $"pr_{Guid.NewGuid():N}",
            Name = $"Test Product {seed}",
            FullName = $"Test Product Full Name {seed}",
            Code = $"{Random.Shared.Next(1, 999999):D6}",
            IsExcised = false,
            ProductTypeId = productType.Id,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return product.Id;
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
