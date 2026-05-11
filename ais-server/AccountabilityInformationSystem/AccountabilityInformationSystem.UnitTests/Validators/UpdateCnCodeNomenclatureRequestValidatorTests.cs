using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes.Update;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Update;
using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public class UpdateCnCodeNomenclatureRequestValidatorTests
{
    private readonly UpdateCnCodeNomenclatureRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestIsValid()
    {
        //Arrange
        UpdateCnCodeNomenclatureRequest request = new() { Code = "12345678", DescriptionBg = "Описание на български език", DescriptionEn = null, IsUsed = false };

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
        UpdateCnCodeNomenclatureRequest request = new() { Code = null, DescriptionBg = "Описание на български език", DescriptionEn = null, IsUsed = false };

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
        UpdateCnCodeNomenclatureRequest request = new() { Code = "", DescriptionBg = "Описание на български език", DescriptionEn = null, IsUsed = false };

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
        UpdateCnCodeNomenclatureRequest request = new() { Code = "12345678", DescriptionBg = null, DescriptionEn = null, IsUsed = false };

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
        UpdateCnCodeNomenclatureRequest request = new() { Code = "12345678", DescriptionBg = "", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

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
        UpdateCnCodeNomenclatureRequest request = new() { Code = code, DescriptionBg = "Описание на български език", DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(UpdateCnCodeNomenclatureRequest.Code), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreaseMaximumLengthDescriptionBg()
    {
        //Arrange
        string description = new('a', EntitiesConstants.CnCodeConstants.DescriptionMaxlength + 1);
        UpdateCnCodeNomenclatureRequest request = new() { Code = "12345678", DescriptionBg = description, DescriptionEn = null, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(UpdateCnCodeNomenclatureRequest.DescriptionBg), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreasemaximumLengthEnDescription()
    {
        //Arrange
        string description = new('a', EntitiesConstants.CnCodeConstants.DescriptionMaxlength + 1);
        UpdateCnCodeNomenclatureRequest request = new() { Code = "12345678", DescriptionBg = "Some description", DescriptionEn = description, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(UpdateCnCodeNomenclatureRequest.DescriptionEn), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }
}
