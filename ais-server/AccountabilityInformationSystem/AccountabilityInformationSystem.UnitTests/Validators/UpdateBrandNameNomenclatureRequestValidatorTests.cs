using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.BrandNames.Update;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Update;
using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public class UpdateBrandNameNomenclatureRequestValidatorTests
{
    private readonly UpdateBrandNameNomenclatureRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestIsValid()
    {
        //Arrange
        UpdateBrandNameNomenclatureRequest request = new() { Code = "A12345", DescriptionBg = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestHasNullCode()
    {
        //Arrange
        UpdateBrandNameNomenclatureRequest request = new() { Code = null, DescriptionBg = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestHasEmptyCode()
    {
        //Arrange
        UpdateBrandNameNomenclatureRequest request = new() { Code = "", DescriptionBg = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestHasNullDescriptionBg()
    {
        //Arrange
        UpdateBrandNameNomenclatureRequest request = new() { Code = "A12345", DescriptionBg = null, DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestHasEmptyDescriptionBg()
    {
        //Arrange
        UpdateBrandNameNomenclatureRequest request = new() { Code = "A12345", DescriptionBg = "", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Theory]
    [InlineData("A1234")]
    [InlineData("A123456")]
    [InlineData("123456")]
    [InlineData("AA1234")]
    [InlineData("AAA123")]
    [InlineData("AAAA12")]
    [InlineData("AAAAA1")]
    [InlineData("AAAAAA")]
    [InlineData("12345A")]
    public async Task Validate_ShouldFailed_WhenInputRequestHasInvalidCode(string code)
    {
        //Arrange
        UpdateBrandNameNomenclatureRequest request = new() { Code = code, DescriptionBg = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(UpdateBrandNameNomenclatureRequest.Code), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreaseMaximumLengthDescriptionBg()
    {
        //Arrange
        string description = new('a', EntitiesConstants.BrandNameConstants.DescriptionMaxlength + 1);
        UpdateBrandNameNomenclatureRequest request = new() { Code = "A12345", DescriptionBg = description, DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(UpdateBrandNameNomenclatureRequest.DescriptionBg), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreasemaximumLengthEnDescription()
    {
        //Arrange
        string description = new('a', EntitiesConstants.BrandNameConstants.DescriptionMaxlength + 1);
        UpdateBrandNameNomenclatureRequest request = new() { Code = "A12345", DescriptionBg = "Some description", DescriptionEn = description, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(UpdateBrandNameNomenclatureRequest.DescriptionEn), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }
}
