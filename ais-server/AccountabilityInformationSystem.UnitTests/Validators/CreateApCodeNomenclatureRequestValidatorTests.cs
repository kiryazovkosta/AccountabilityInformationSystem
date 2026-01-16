using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Models.ExciseNomenclatures;
using AccountabilityInformationSystem.Api.Models.ExciseNomenclatures.ApCodes;
using FluentValidation.Results;
using System.Linq;
using System.Threading.Tasks;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public class CreateApCodeNomenclatureRequestValidatorTests
{
    private readonly CreateApCodeNomenclatureRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestIsValid()
    {
        //Arrange
        CreateApCodeNomenclatureRequest request = new(){ Code = "A100", BgDescription = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("A12")]
    [InlineData("12345")]
    [InlineData("A1234")]
    [InlineData("1234")]
    [InlineData("AA12")]
    [InlineData("AAA1")]
    [InlineData("AAAA")]
    public async Task Validate_ShouldFailed_WhenInputRequestHasInvalidCode(string code)
    {
        //Arrange
        CreateApCodeNomenclatureRequest request = new() { Code = code, BgDescription = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateApCodeNomenclatureRequest.Code), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestHasEmptyBgDescription()
    {
        //Arrange
        CreateApCodeNomenclatureRequest request = new() { Code = "A100", BgDescription = "", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateApCodeNomenclatureRequest.BgDescription), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreaseMaximumLengthBgDescription()
    {
        //Arrange
        string description = new('a', EntitiesConstants.ApCodeConstants.DescriptionMaxlength + 1);
        CreateApCodeNomenclatureRequest request = new() { Code = "A100", BgDescription = description, DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateApCodeNomenclatureRequest.BgDescription), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreasemaximumLengthEnDescription()
    {
        //Arrange
        string description = new('a', EntitiesConstants.ApCodeConstants.DescriptionMaxlength + 1);
        CreateApCodeNomenclatureRequest request = new() { Code = "A100", BgDescription = "Some description", DescriptionEn = description, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateApCodeNomenclatureRequest.DescriptionEn), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }
}
