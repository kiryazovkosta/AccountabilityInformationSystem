using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Family;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class GetWarrantyRecordsTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    [Theory]
    [InlineData("model")]
    [InlineData("purchaseDate")]
    [InlineData("warrantyBrandName")]
    [InlineData("receiptExists")]
    [InlineData("frontImageExists")]
    [InlineData("backImageExists")]
    [InlineData("duration")]
    [InlineData("endDate")]
    [InlineData("status desc")]
    public async Task GetWarrantyRecords_ShouldSucceed_ForEverySortableField(string sort)
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string token = Guid.NewGuid().ToString("N");
        await SeedWarrantyRecordAsync($"Brand {token}", $"Model {token}", durationInMonths: 12);

        // Act
        HttpResponseMessage response = await client.GetAsync(
            $"{Routes.WarrantyRecords.Base}?q={token}&sort={Uri.EscapeDataString(sort)}",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetWarrantyRecords_ShouldOrderByBrandName_WhenSortedByWarrantyBrandName()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string token = Guid.NewGuid().ToString("N");
        await SeedWarrantyRecordAsync($"Zulu {token}", $"Model {token} 1", durationInMonths: 12);
        await SeedWarrantyRecordAsync($"Alpha {token}", $"Model {token} 2", durationInMonths: 12);

        // Act
        List<string> ascending = await GetItemValuesAsync(client, token, "warrantyBrandName", "warrantyBrandName");
        List<string> descending = await GetItemValuesAsync(client, token, "warrantyBrandName desc", "warrantyBrandName");

        // Assert
        Assert.Equal([$"Alpha {token}", $"Zulu {token}"], ascending);
        Assert.Equal([$"Zulu {token}", $"Alpha {token}"], descending);
    }

    [Fact]
    public async Task GetWarrantyRecords_ShouldOrderByEndDate_WhenSortedByEndDate()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string token = Guid.NewGuid().ToString("N");
        await SeedWarrantyRecordAsync($"Brand {token}", $"Short {token}", durationInMonths: 12);
        await SeedWarrantyRecordAsync($"Brand {token}", $"Long {token}", durationInMonths: 36);

        // Act
        List<string> ascending = await GetItemValuesAsync(client, token, "endDate", "model");
        List<string> descending = await GetItemValuesAsync(client, token, "endDate desc", "model");

        // Assert
        Assert.Equal([$"Short {token}", $"Long {token}"], ascending);
        Assert.Equal([$"Long {token}", $"Short {token}"], descending);
    }

    private static async Task<List<string>> GetItemValuesAsync(HttpClient client, string token, string sort, string property)
    {
        HttpResponseMessage response = await client.GetAsync(
            $"{Routes.WarrantyRecords.Base}?q={token}&sort={Uri.EscapeDataString(sort)}",
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        JsonElement page = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        return [.. page.GetProperty("items").EnumerateArray().Select(item => item.GetProperty(property).GetString()!)];
    }

    private async Task SeedWarrantyRecordAsync(string brandName, string model, int durationInMonths)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        WarrantyBrand warrantyBrand = new()
        {
            Id = $"wb_{Guid.NewGuid():N}",
            Name = brandName,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow
        };

        WarrantyRecord warrantyRecord = new()
        {
            Id = $"wr_{Guid.NewGuid():N}",
            WarrantyBrand = warrantyBrand,
            Model = model,
            PurchaseDate = new DateOnly(2024, 1, 1),
            Duration = durationInMonths,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow
        };

        dbContext.WarrantyRecords.Add(warrantyRecord);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
