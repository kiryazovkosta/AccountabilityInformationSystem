using System;
using System.Net;
using AccountabilityInformationSystem.Api.Domain.Entities.Common;
using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Family;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class DeleteWarrantyRecordTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    [Fact]
    public async Task DeleteWarrantyRecord_ShouldSuccess_WhenRecordExists()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string warrantyBrandId = await CreateWarrantyBrandAsync();
        string warrantyRecordId = await CreateWarrantyRecordAsync(warrantyBrandId);

        // Act
        HttpResponseMessage response = await client.DeleteAsync(
            $"{Routes.WarrantyRecords.Base}/{warrantyRecordId}",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        WarrantyRecord? deletedRecord = await dbContext.WarrantyRecords
            .FirstOrDefaultAsync(w => w.Id == warrantyRecordId, TestContext.Current.CancellationToken);

        Assert.Null(deletedRecord);
    }

    [Fact]
    public async Task DeleteWarrantyRecord_ShouldFail_WhenRecordDoesNotExist()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string nonExistingWarrantyRecordId = $"wr_{Guid.NewGuid():N}";

        // Act
        HttpResponseMessage response = await client.DeleteAsync(
            $"{Routes.WarrantyRecords.Base}/{nonExistingWarrantyRecordId}",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteWarrantyRecord_ShouldDeleteAssociatedStorageFile_WhenRecordHasReceipt()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string warrantyBrandId = await CreateWarrantyBrandAsync();
        string storageFileId = await CreateStorageFileAsync();
        string warrantyRecordId = await CreateWarrantyRecordAsync(warrantyBrandId, receiptId: storageFileId);

        // Act
        HttpResponseMessage response = await client.DeleteAsync(
            $"{Routes.WarrantyRecords.Base}/{warrantyRecordId}",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        StorageFile? deletedStorageFile = await dbContext.StorageFiles
            .FirstOrDefaultAsync(f => f.Id == storageFileId, TestContext.Current.CancellationToken);

        Assert.Null(deletedStorageFile);
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

    private async Task<string> CreateStorageFileAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        StorageFile storageFile = new()
        {
            Id = $"sf_{Guid.NewGuid():N}",
            BlobName = $"fake_{Guid.NewGuid():N}.pdf",
            IsPrivate = true,
            OriginalFileName = "receipt.pdf",
            ContentType = "application/pdf",
            SizeBytes = 1024,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow
        };

        dbContext.StorageFiles.Add(storageFile);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return storageFile.Id;
    }

    private async Task<string> CreateWarrantyRecordAsync(string warrantyBrandId, string? receiptId = null)
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
            ReceiptId = receiptId,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow
        };

        dbContext.WarrantyRecords.Add(warrantyRecord);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return warrantyRecord.Id;
    }
}
