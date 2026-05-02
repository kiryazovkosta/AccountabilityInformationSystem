using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.BrandNames.Create;
using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public class CreateBrandNameNomenclatureRequestValidatorTests
{
    private readonly CreateBrandNameNomenclatureRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenInputRequestIsValid()
    {
        //Arrange
        CreateBrandNameNomenclatureRequest request = new(){ Code = "A12345", DescriptionBg = "Описание на български език", DescriptionEn = "English description", IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

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
        CreateBrandNameNomenclatureRequest request = new() { Code = code, DescriptionBg = "Описание на български език", DescriptionEn = "English description", IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateBrandNameNomenclatureRequest.Code), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestHasEmptyDescriptionBg()
    {
        //Arrange
        CreateBrandNameNomenclatureRequest request = new() { Code = "12345678", DescriptionBg = "", DescriptionEn = "English description", IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateBrandNameNomenclatureRequest.DescriptionBg), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreaseMaximumLengthDescriptionBg()
    {
        //Arrange
        string description = new('a', EntitiesConstants.CnCodeConstants.DescriptionMaxlength + 1);
        CreateBrandNameNomenclatureRequest request = new() { Code = "A12345", DescriptionBg = description, DescriptionEn = "English description", IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateBrandNameNomenclatureRequest.DescriptionBg), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestHasEmptyDescriptionEn()
    {
        //Arrange
        CreateBrandNameNomenclatureRequest request = new() { Code = "A12345", DescriptionBg = "Some description", DescriptionEn = "", IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateBrandNameNomenclatureRequest.DescriptionEn), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }

    [Fact]
    public async Task Validate_ShouldFailed_WhenInputRequestIncreasemaximumLengthEnDescription()
    {
        //Arrange
        string description = new('a', EntitiesConstants.BrandNameConstants.DescriptionMaxlength + 1);
        CreateBrandNameNomenclatureRequest request = new() { Code = "A12345", DescriptionBg = "Some description", DescriptionEn = description, IsUsed = false };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(nameof(CreateBrandNameNomenclatureRequest.DescriptionEn), validationResult.Errors.Select(e => e.PropertyName).ToList());
    }
}
