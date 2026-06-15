using System;
using System.Collections.Generic;
using System.Text;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.BrandNames.Create;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Create;
using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Time.Testing;
using Xunit.Sdk;

namespace AccountabilityInformationSystem.UnitTests.Validators.Family;

public class CreateWarrantyRecordRequestValidatorTests
{
    private readonly CreateWarrantyRecordRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestIsValidWithoutFiles()
    {
        // Arrange
        string brandId = "wb_019eca4b-fd91-7b37-94f6-9555be5af9cb";
        DateTimeOffset frozenTime = new(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        FakeTimeProvider timeProvider = new(frozenTime);

        CreateWarrantyRecordRequest request = new() {
            Duration = 12,
            Model = "X-One",
            PurchaseDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().Date),
            WarrantyBrandId = brandId
        };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData(nameof(CreateWarrantyRecordRequest.Receipt), null, null)]
    [InlineData(nameof(CreateWarrantyRecordRequest.Receipt), nameof(CreateWarrantyRecordRequest.FrontImage), null)]
    [InlineData(null, nameof(CreateWarrantyRecordRequest.FrontImage), null)]
    [InlineData(null, nameof(CreateWarrantyRecordRequest.FrontImage), nameof(CreateWarrantyRecordRequest.BackImage))]
    [InlineData(null, null, nameof(CreateWarrantyRecordRequest.BackImage))]
    [InlineData(nameof(CreateWarrantyRecordRequest.Receipt), nameof(CreateWarrantyRecordRequest.FrontImage), nameof(CreateWarrantyRecordRequest.BackImage))]
    public async Task Validate_ShouldSuccess_WhenInputRequestIsValidWithFiles(string? receipt, string? front, string? back)
    {
        // Arrange
        string brandId = "wb_019eca4b-fd91-7b37-94f6-9555be5af9cb";
        DateTimeOffset frozenTime = new(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        FakeTimeProvider timeProvider = new(frozenTime);

        CreateWarrantyRecordRequest request = new()
        {
            Duration = 12,
            Model = "X-One",
            PurchaseDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().Date),
            WarrantyBrandId = brandId,
            Receipt = CreateFackFile(receipt),
            FrontImage = CreateFackFile(front),
            BackImage = CreateFackFile(back)
        };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123456789012345678901234567890123456789012345678901")]
    public async Task Validate_ShouldFailed_WhenWarrantuBrandIsIsNotValid(string warrantyBrandId)
    {
        // Arrange
        string brandId = warrantyBrandId;
        DateTimeOffset frozenTime = new(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        FakeTimeProvider timeProvider = new(frozenTime);

        CreateWarrantyRecordRequest request = new()
        {
            Duration = 12,
            Model = "X-One",
            PurchaseDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().Date),
            WarrantyBrandId = brandId
        };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateWarrantyRecordRequest.WarrantyBrandId), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(EntitiesConstants.WarrantyRecord.ModelMaxLength + 1)]
    public async Task Validate_ShouldFailed_ModelIsNotValid(int modelLength)
    {
        // Arrange
        string brandId = "wb_019eca4b-fd91-7b37-94f6-9555be5af9cb";
        string model = modelLength == 0 ? "" : new string('s', modelLength);
        DateTimeOffset frozenTime = new(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        FakeTimeProvider timeProvider = new(frozenTime);

        CreateWarrantyRecordRequest request = new()
        {
            Duration = 12,
            Model = model,
            PurchaseDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().Date),
            WarrantyBrandId = brandId
        };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateWarrantyRecordRequest.Model), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenPurchaseDateIsEmpty()
    {
        // Arrange
        string brandId = "wb_019eca4b-fd91-7b37-94f6-9555be5af9cb";

        CreateWarrantyRecordRequest request = new()
        {
            Duration = 12,
            Model = "X-One",
            PurchaseDate = default,
            WarrantyBrandId = brandId
        };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateWarrantyRecordRequest.PurchaseDate), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Theory]
    [InlineData(nameof(CreateWarrantyRecordRequest.Receipt))]
    [InlineData(nameof(CreateWarrantyRecordRequest.FrontImage))]
    [InlineData(nameof(CreateWarrantyRecordRequest.BackImage))]
    public async Task Validate_ShouldFailed_WhenFileHasInvalidExtension(string fileProperty)
    {
        // Arrange
        string brandId = "wb_019eca4b-fd91-7b37-94f6-9555be5af9cb";
        DateTimeOffset frozenTime = new(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        FakeTimeProvider timeProvider = new(frozenTime);

        using MemoryStream stream = new([0x1]);
        IFormFile invalidFile = new FormFile(stream, 0, stream.Length, fileProperty, "malware.exe")
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/octet-stream"
        };

        CreateWarrantyRecordRequest request = new()
        {
            Duration = 12,
            Model = "X-One",
            PurchaseDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().Date),
            WarrantyBrandId = brandId,
            Receipt = fileProperty == nameof(CreateWarrantyRecordRequest.Receipt) ? invalidFile : null,
            FrontImage = fileProperty == nameof(CreateWarrantyRecordRequest.FrontImage) ? invalidFile : null,
            BackImage = fileProperty == nameof(CreateWarrantyRecordRequest.BackImage) ? invalidFile : null
        };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(fileProperty, validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Theory]
    [InlineData(nameof(CreateWarrantyRecordRequest.Receipt))]
    [InlineData(nameof(CreateWarrantyRecordRequest.FrontImage))]
    [InlineData(nameof(CreateWarrantyRecordRequest.BackImage))]
    public async Task Validate_ShouldFailed_WhenFileHasInvalidSize(string fileProperty)
    {
        // Arrange
        string brandId = "wb_019eca4b-fd91-7b37-94f6-9555be5af9cb";
        DateTimeOffset frozenTime = new(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        FakeTimeProvider timeProvider = new(frozenTime);

        byte[] buffer = new byte[2 * 1024 * 1024 + 1];
        using MemoryStream stream = new(buffer);
        IFormFile invalidFile = new FormFile(stream, 0, stream.Length, fileProperty, "image.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        CreateWarrantyRecordRequest request = new()
        {
            Duration = 12,
            Model = "X-One",
            PurchaseDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().Date),
            WarrantyBrandId = brandId,
            Receipt = fileProperty == nameof(CreateWarrantyRecordRequest.Receipt) ? invalidFile : null,
            FrontImage = fileProperty == nameof(CreateWarrantyRecordRequest.FrontImage) ? invalidFile : null,
            BackImage = fileProperty == nameof(CreateWarrantyRecordRequest.BackImage) ? invalidFile : null
        };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(fileProperty, validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_ShouldFailed_DurationIsNotValid(int duration)
    {
        // Arrange
        string brandId = "wb_019eca4b-fd91-7b37-94f6-9555be5af9cb";
        string model = "X-One";
        DateTimeOffset frozenTime = new(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        FakeTimeProvider timeProvider = new(frozenTime);

        CreateWarrantyRecordRequest request = new()
        {
            Duration = duration,
            Model = model,
            PurchaseDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().Date),
            WarrantyBrandId = brandId
        };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateWarrantyRecordRequest.Duration), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    private static IFormFile? CreateFackFile(string? fileProperty)
    {
        if (fileProperty is null)
        {
            return null;
        }

        byte[] buffer = new byte[2 * 1024 * 14];
        using MemoryStream stream = new(buffer);
        IFormFile invalidFile = new FormFile(stream, 0, stream.Length, fileProperty, "image.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        return invalidFile;
    }
}
