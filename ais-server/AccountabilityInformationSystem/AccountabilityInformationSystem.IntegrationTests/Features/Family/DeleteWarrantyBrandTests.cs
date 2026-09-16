using System;
using System.Net;
using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Family;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class DeleteWarrantyBrandTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    [Fact]
    public async Task DeleteWarrantyBrand_ShouldSucceed_WhenBrandHasNoWarrantyRecords()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string warrantyBrandId = await SeedWarrantyBrandAsync();

        // Act
        HttpResponseMessage response = await client.DeleteAsync(
            Routes.WarrantyBrands.Delete(warrantyBrandId),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteWarrantyBrand_ShouldFail_WhenBrandIsStillUsedByWarrantyRecord()
    {
        // Arrange — this is the "still used by existing warranty records and cannot be deleted" guard rule.
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string warrantyBrandId = await SeedWarrantyBrandAsync();
        await SeedWarrantyRecordAsync(warrantyBrandId);

        // Act
        HttpResponseMessage response = await client.DeleteAsync(
            Routes.WarrantyBrands.Delete(warrantyBrandId),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task DeleteWarrantyBrand_ShouldFail_WhenBrandDoesNotExist()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string nonExistingWarrantyBrandId = $"wb_{Guid.NewGuid():N}";

        // Act
        HttpResponseMessage response = await client.DeleteAsync(
            Routes.WarrantyBrands.Delete(nonExistingWarrantyBrandId),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<string> SeedWarrantyBrandAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        WarrantyBrand warrantyBrand = new()
        {
            Id = $"wb_{Guid.NewGuid():N}",
            Name = $"Test Brand {Guid.NewGuid():N}",
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow
        };

        dbContext.WarrantyBrands.Add(warrantyBrand);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return warrantyBrand.Id;
    }

    private async Task SeedWarrantyRecordAsync(string warrantyBrandId)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        WarrantyRecord warrantyRecord = new()
        {
            Id = $"wr_{Guid.NewGuid():N}",
            WarrantyBrandId = warrantyBrandId,
            Model = "WH01",
            PurchaseDate = DateOnly.FromDateTime(DateTime.Today),
            Duration = 24,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow
        };

        dbContext.WarrantyRecords.Add(warrantyRecord);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
