using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes.CreateCnCode;
using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation.Results;
using System.Linq;
using System.Threading.Tasks;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public class CreateCnCodeNomenclatureRequestValidatorTests
{
    private readonly CreateCnCodeNomenclatureRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestIsValid()
    {
        //Arrange
        CreateCnCodeNomenclatureRequest request = new(){ Code = "12345678", BgDescription = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Theory]
    [InlineData("")]
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
        CreateCnCodeNomenclatureRequest request = new() { Code = code, BgDescription = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateCnCodeNomenclatureRequest.Code), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestHasEmptyBgDescription()
    {
        //Arrange
        CreateCnCodeNomenclatureRequest request = new() { Code = "12345678", BgDescription = "", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateCnCodeNomenclatureRequest.BgDescription), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreaseMaximumLengthBgDescription()
    {
        //Arrange
        string description = new('a', EntitiesConstants.CnCodeConstants.DescriptionMaxlength + 1);
        CreateCnCodeNomenclatureRequest request = new() { Code = "12345678", BgDescription = description, DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateCnCodeNomenclatureRequest.BgDescription), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreasemaximumLengthEnDescription()
    {
        //Arrange
        string description = new('a', EntitiesConstants.CnCodeConstants.DescriptionMaxlength + 1);
        CreateCnCodeNomenclatureRequest request = new() { Code = "12345678", BgDescription = "Some description", DescriptionEn = description, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateCnCodeNomenclatureRequest.DescriptionEn), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }
}
