using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.ApCodes.Create;
using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public class CreateApCodeNomenclatureRequestValidatorTests
{
    private readonly CreateApCodeNomenclatureRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestIsValid()
    {
        //Arrange
        CreateApCodeNomenclatureRequest request = new(){ Code = "A100", DescriptionBg = "Описание на български език", DescriptionEn = "English description", IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellation: CancellationToken.None);

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
        CreateApCodeNomenclatureRequest request = new() { Code = code, DescriptionBg = "Описание на български език", DescriptionEn = "English description", IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateApCodeNomenclatureRequest.Code), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestHasEmptyDescriptionBg()
    {
        //Arrange
        CreateApCodeNomenclatureRequest request = new() { Code = "A100", DescriptionBg = "", DescriptionEn = "English description", IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateApCodeNomenclatureRequest.DescriptionBg), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreaseMaximumLengthDescriptionBg()
    {
        //Arrange
        string description = new('a', EntitiesConstants.ApCodeConstants.DescriptionMaxlength + 1);
        CreateApCodeNomenclatureRequest request = new() { Code = "A100", DescriptionBg = description, DescriptionEn = "English description", IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateApCodeNomenclatureRequest.DescriptionBg), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestHasEmptyDescriptionEn()
    {
        //Arrange
        CreateApCodeNomenclatureRequest request = new() { Code = "A100", DescriptionBg = "Some description", DescriptionEn = "", IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateApCodeNomenclatureRequest.DescriptionEn), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreasemaximumLengthEnDescription()
    {
        //Arrange
        string description = new('a', EntitiesConstants.ApCodeConstants.DescriptionMaxlength + 1);
        CreateApCodeNomenclatureRequest request = new() { Code = "A100", DescriptionBg = "Some description", DescriptionEn = description, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateApCodeNomenclatureRequest.DescriptionEn), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }
}
