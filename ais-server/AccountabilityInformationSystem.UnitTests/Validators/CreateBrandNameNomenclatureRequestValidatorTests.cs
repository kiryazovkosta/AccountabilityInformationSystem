using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.BrandNames.CreateBrandName;
using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation.Results;
using System.Linq;
using System.Threading.Tasks;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public class CreateBrandNameNomenclatureRequestValidatorTests
{
    private readonly CreateBrandNameNomenclatureRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestIsValid()
    {
        //Arrange
        CreateBrandNameNomenclatureRequest request = new(){ Code = "A12345", BgDescription = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Theory]
    [InlineData("")]
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
        CreateBrandNameNomenclatureRequest request = new() { Code = code, BgDescription = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateBrandNameNomenclatureRequest.Code), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestHasEmptyBgDescription()
    {
        //Arrange
        CreateBrandNameNomenclatureRequest request = new() { Code = "12345678", BgDescription = "", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateBrandNameNomenclatureRequest.BgDescription), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreaseMaximumLengthBgDescription()
    {
        //Arrange
        string description = new('a', EntitiesConstants.CnCodeConstants.DescriptionMaxlength + 1);
        CreateBrandNameNomenclatureRequest request = new() { Code = "A12345", BgDescription = description, DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateBrandNameNomenclatureRequest.BgDescription), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreasemaximumLengthEnDescription()
    {
        //Arrange
        string description = new('a', EntitiesConstants.BrandNameConstants.DescriptionMaxlength + 1);
        CreateBrandNameNomenclatureRequest request = new() { Code = "A12345", BgDescription = "Some description", DescriptionEn = description, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateBrandNameNomenclatureRequest.DescriptionEn), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }
}
