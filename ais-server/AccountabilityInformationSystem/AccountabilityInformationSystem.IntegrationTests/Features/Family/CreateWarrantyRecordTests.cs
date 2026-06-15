using System;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Create;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Family;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class CreateWarrantyRecordTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    [Fact]
    public async Task CreateWarrantyRecord_ShouldSuccess_WithValidParameters()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string warrantyBrandId = await CreateWarrantyBrandAsync();

        using StringContent warrantyBrandIdContent = new(warrantyBrandId);
        using StringContent modelContent = new("WH01");
        using StringContent durationContent = new("24");
        using StringContent purchaseDateContent = new(DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

        using MultipartFormDataContent content = new()
        {
            { warrantyBrandIdContent, nameof(CreateWarrantyRecordRequest.WarrantyBrandId) },
            { modelContent, nameof(CreateWarrantyRecordRequest.Model) },
            { durationContent, nameof(CreateWarrantyRecordRequest.Duration) },
            { purchaseDateContent, nameof(CreateWarrantyRecordRequest.PurchaseDate) }
        };

        // Act
        HttpResponseMessage response = await client.PostAsync(
            Routes.WarrantyRecords.Base,
            content,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateWarrantyRecord_ShouldFailed_WithInvalidParameters()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string warrantyBrandId = await CreateWarrantyBrandAsync();

        using StringContent warrantyBrandIdContent = new(warrantyBrandId);
        using StringContent modelContent = new(string.Empty);
        using StringContent durationContent = new("0");
        using StringContent purchaseDateContent = new(DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

        using MultipartFormDataContent content = new()
        {
            { warrantyBrandIdContent, nameof(CreateWarrantyRecordRequest.WarrantyBrandId) },
            { modelContent, nameof(CreateWarrantyRecordRequest.Model) },
            { durationContent, nameof(CreateWarrantyRecordRequest.Duration) },
            { purchaseDateContent, nameof(CreateWarrantyRecordRequest.PurchaseDate) }
        };

        // Act
        HttpResponseMessage response = await client.PostAsync(
            Routes.WarrantyRecords.Base,
            content,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateWarrantyRecord_ShouldFailed_WhenWarrantyBrandDoesNotExist()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string nonExistingWarrantyBrandId = $"wb_{Guid.NewGuid():N}";

        using StringContent warrantyBrandIdContent = new(nonExistingWarrantyBrandId);
        using StringContent modelContent = new("WH01");
        using StringContent durationContent = new("24");
        using StringContent purchaseDateContent = new(DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

        using MultipartFormDataContent content = new()
        {
            { warrantyBrandIdContent, nameof(CreateWarrantyRecordRequest.WarrantyBrandId) },
            { modelContent, nameof(CreateWarrantyRecordRequest.Model) },
            { durationContent, nameof(CreateWarrantyRecordRequest.Duration) },
            { purchaseDateContent, nameof(CreateWarrantyRecordRequest.PurchaseDate) }
        };

        // Act
        HttpResponseMessage response = await client.PostAsync(
            Routes.WarrantyRecords.Base,
            content,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateWarrantyRecord_ShouldFailed_WithInvalidFileExtension()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string warrantyBrandId = await CreateWarrantyBrandAsync();

        using StringContent warrantyBrandIdContent = new(warrantyBrandId);
        using StringContent modelContent = new("WH01");
        using StringContent durationContent = new("24");
        using StringContent purchaseDateContent = new(DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        using ByteArrayContent receiptContent = new([0x1]);
        receiptContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        using MultipartFormDataContent content = new()
        {
            { warrantyBrandIdContent, nameof(CreateWarrantyRecordRequest.WarrantyBrandId) },
            { modelContent, nameof(CreateWarrantyRecordRequest.Model) },
            { durationContent, nameof(CreateWarrantyRecordRequest.Duration) },
            { purchaseDateContent, nameof(CreateWarrantyRecordRequest.PurchaseDate) },
            { receiptContent, nameof(CreateWarrantyRecordRequest.Receipt), "malware.exe" }
        };

        // Act
        HttpResponseMessage response = await client.PostAsync(
            Routes.WarrantyRecords.Base,
            content,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<string> CreateWarrantyBrandAsync()
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
}
