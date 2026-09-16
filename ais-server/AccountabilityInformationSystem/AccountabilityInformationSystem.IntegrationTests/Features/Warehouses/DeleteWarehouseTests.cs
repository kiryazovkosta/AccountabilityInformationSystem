using System;
using System.Net;
using System.Net.Http.Json;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Warehouses.Create;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Warehouses;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class DeleteWarehouseTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    [Fact]
    public async Task DeleteWarehouse_ShouldSucceed_WhenWarehouseHasNoIkunks()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string warehouseId = await CreateWarehouseAsync(client);

        // Act
        HttpResponseMessage response = await client.DeleteAsync(
            Routes.Warehouses.Delete(warehouseId),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteWarehouse_ShouldFail_WhenWarehouseHasAssociatedIkunks()
    {
        // Arrange — this is the "Cannot delete warehouse with associated ikunks!" guard rule.
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string warehouseId = await CreateWarehouseAsync(client);
        await SeedIkunkAsync(warehouseId);

        // Act
        HttpResponseMessage response = await client.DeleteAsync(
            Routes.Warehouses.Delete(warehouseId),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteWarehouse_ShouldFail_WhenWarehouseDoesNotExist()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string nonExistingWarehouseId = $"wh_{Guid.NewGuid():N}";

        // Act
        HttpResponseMessage response = await client.DeleteAsync(
            Routes.Warehouses.Delete(nonExistingWarehouseId),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static async Task<string> CreateWarehouseAsync(HttpClient client)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Warehouses.Base,
            new CreateWarehouseRequest
            {
                Name = $"WH-DEL-{Guid.NewGuid():N}",
                FullName = "Warehouse for delete tests",
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

    private async Task SeedIkunkAsync(string warehouseId)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Ikunk ikunk = new()
        {
            Id = $"ik_{Guid.NewGuid():N}",
            Name = $"Test Ikunk {Guid.NewGuid():N}",
            FullName = "Test Ikunk",
            OrderPosition = 1,
            ActiveFrom = new DateOnly(2024, 1, 1),
            ActiveTo = new DateOnly(2099, 12, 31),
            WarehouseId = warehouseId,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Ikunks.Add(ikunk);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
