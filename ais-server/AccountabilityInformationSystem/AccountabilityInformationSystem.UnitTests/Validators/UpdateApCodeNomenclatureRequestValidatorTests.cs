using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.ApCodes.UpdateApCode;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.UpdateExciseNomenclature;
using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public class UpdateApCodeNomenclatureRequestValidatorTests
{
    private readonly UpdateApCodeNomenclatureRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestIsValid()
    {
        //Arrange
        UpdateApCodeNomenclatureRequest request = new() { Code = "A100", BgDescription = "Описание на български език", DescriptionEn = null, IsUsed = false };

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
        UpdateApCodeNomenclatureRequest request = new() { Code = null, BgDescription = "Описание на български език", DescriptionEn = null, IsUsed = false };

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
        UpdateApCodeNomenclatureRequest request = new() { Code = "", BgDescription = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestHasNullBgDescription()
    {
        //Arrange
        UpdateApCodeNomenclatureRequest request = new() { Code = "A100", BgDescription = null, DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestHasEmptyBgDescription()
    {
        //Arrange
        UpdateApCodeNomenclatureRequest request = new() { Code = "A100", BgDescription = "", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Theory]
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
        UpdateApCodeNomenclatureRequest request = new() { Code = code, BgDescription = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(UpdateApCodeNomenclatureRequest.Code), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreaseMaximumLengthBgDescription()
    {
        //Arrange
        string description = new('a', EntitiesConstants.ApCodeConstants.DescriptionMaxlength + 1);
        UpdateApCodeNomenclatureRequest request = new() { Code = "A100", BgDescription = description, DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(UpdateApCodeNomenclatureRequest.BgDescription), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreasemaximumLengthEnDescription()
    {
        //Arrange
        string description = new('a', EntitiesConstants.ApCodeConstants.DescriptionMaxlength + 1);
        UpdateApCodeNomenclatureRequest request = new() { Code = "A100", BgDescription = "Some description", DescriptionEn = description, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(UpdateApCodeNomenclatureRequest.DescriptionEn), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }
}
