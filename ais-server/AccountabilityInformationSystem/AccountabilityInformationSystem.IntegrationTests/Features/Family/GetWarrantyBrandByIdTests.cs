using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Family;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class GetWarrantyBrandByIdTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    [Fact]
    public async Task GetWarrantyBrandById_ShouldIncludeTheModelsOfItsWarranties()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string model = $"Model {Guid.NewGuid():N}";
        string warrantyBrandId = await SeedWarrantyBrandWithRecordAsync(model);

        // Act
        HttpResponseMessage response = await client.GetAsync(
            $"{Routes.WarrantyBrands.Base}/{warrantyBrandId}",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonElement warrantyBrand = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        JsonElement warranty = Assert.Single(warrantyBrand.GetProperty("warranties").EnumerateArray());
        Assert.Equal(model, warranty.GetString());
    }

    private async Task<string> SeedWarrantyBrandWithRecordAsync(string model)
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

        dbContext.WarrantyRecords.Add(new WarrantyRecord
        {
            Id = $"wr_{Guid.NewGuid():N}",
            WarrantyBrand = warrantyBrand,
            Model = model,
            PurchaseDate = DateOnly.FromDateTime(DateTime.Today),
            Duration = 24,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return warrantyBrand.Id;
    }
}
