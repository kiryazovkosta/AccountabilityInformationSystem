using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes.UpdateCnCode;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.UpdateExciseNomenclature;
using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation.Results;
using System.Linq;
using System.Threading.Tasks;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public class UpdateCnCodeNomenclatureRequestValidatorTests
{
    private readonly UpdateCnCodeNomenclatureRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestIsValid()
    {
        //Arrange
        UpdateCnCodeNomenclatureRequest request = new() { Code = "12345678", BgDescription = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestHasNullCode()
    {
        //Arrange
        UpdateCnCodeNomenclatureRequest request = new() { Code = null, BgDescription = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestHasEmptyCode()
    {
        //Arrange
        UpdateCnCodeNomenclatureRequest request = new() { Code = "", BgDescription = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestHasNullBgDescription()
    {
        //Arrange
        UpdateCnCodeNomenclatureRequest request = new() { Code = "12345678", BgDescription = null, DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestHasEmptyBgDescription()
    {
        //Arrange
        UpdateCnCodeNomenclatureRequest request = new() { Code = "12345678", BgDescription = "", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1234567")]
    [InlineData("A123456")]
    [InlineData("123456789")]
    [InlineData("A12345678")]
    [InlineData("A1234567")]
    [InlineData("AA123456")]
    [InlineData("1234567A")]
    [InlineData("AAAAAAAA")]
    public async Task Validate_ShouldFailed_WhenInputRequestHasInvalidCode(string code)
    {
        //Arrange
        UpdateCnCodeNomenclatureRequest request = new() { Code = code, BgDescription = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(UpdateCnCodeNomenclatureRequest.Code), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreaseMaximumLengthBgDescription()
    {
        //Arrange
        string description = new('a', EntitiesConstants.CnCodeConstants.DescriptionMaxlength + 1);
        UpdateCnCodeNomenclatureRequest request = new() { Code = "12345678", BgDescription = description, DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(UpdateCnCodeNomenclatureRequest.BgDescription), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreasemaximumLengthEnDescription()
    {
        //Arrange
        string description = new('a', EntitiesConstants.CnCodeConstants.DescriptionMaxlength + 1);
        UpdateCnCodeNomenclatureRequest request = new() { Code = "12345678", BgDescription = "Some description", DescriptionEn = description, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(UpdateCnCodeNomenclatureRequest.DescriptionEn), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }
}
