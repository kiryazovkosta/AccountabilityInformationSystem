using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AccountabilityInformationSystem.Api.Features.Warehouses.Create;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Warehouses;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class CreateWarehouseTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    [Fact]
    public async Task CreateWarehouse_ShouldSuccess_WithValidParameters()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        CreateWarehouseRequest request = new()
        {
            Name = "WH01",
            FullName = "Warehouse 01",
            Description = "Test warehouse",
            OrderPosition = 1,
            ExciseNumber = "BGNCA00001234",
            ActiveFrom = new DateOnly(2024, 1, 1),
            ActiveTo = new DateOnly(2099, 12, 31)
        };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Warehouses.Base,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateWarehouse_ShouldFailed_WithInvalidParameters()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        CreateWarehouseRequest request = new()
        {
            Name = "WH01",
            FullName = "Warehouse 01",
            Description = "Test warehouse",
            OrderPosition = 1,
            ExciseNumber = "BGNCA0000",
            ActiveFrom = new DateOnly(2024, 1, 1),
            ActiveTo = new DateOnly(2099, 12, 31)
        };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Warehouses.Base,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateWarehouse_ShouldReturnValidationProblem_WithFieldErrors()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        CreateWarehouseRequest request = new()
        {
            Name = "WH03",
            FullName = "Warehouse 03",
            OrderPosition = 1,
            ExciseNumber = "BGNCA0000",
            ActiveFrom = new DateOnly(2024, 1, 1),
            ActiveTo = new DateOnly(2099, 12, 31)
        };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Warehouses.Base,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        JsonElement problem = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        Assert.Equal("Validation Error", problem.GetProperty("title").GetString());
        Assert.True(problem.GetProperty("errors").TryGetProperty("excisenumber", out _));
    }

    [Fact]
    public async Task CreateWarehouse_ShouldFailed_WithWahehouseNameIsNotUnique()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        CreateWarehouseRequest request = new()
        {
            Name = "WH02",
            FullName = "Warehouse 02",
            Description = "Test warehouse",
            OrderPosition = 1,
            ExciseNumber = "BGNCA00004321",
            ActiveFrom = new DateOnly(2024, 1, 1),
            ActiveTo = new DateOnly(2099, 12, 31)
        };

        // Act
        await client.PostAsJsonAsync(
            Routes.Warehouses.Base,
            request,
            TestContext.Current.CancellationToken);

        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Warehouses.Base,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
