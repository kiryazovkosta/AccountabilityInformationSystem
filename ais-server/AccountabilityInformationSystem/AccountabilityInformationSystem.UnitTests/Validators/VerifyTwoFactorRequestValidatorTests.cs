using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.VerifyTwoFactor;
using FluentValidation.Results;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public sealed class VerifyTwoFactorRequestValidatorTests
{
    private readonly VerifyTwoFactorRequestValidator _validator = new();

    private static VerifyTwoFactorRequest ValidRequest() => new()
    {
        SetupToken = "valid-setup-token",
        Code = "123456"
    };

    [Fact]
    public async Task Validate_ShouldSuccess_WhenAllFieldsAreValid()
    {
        ValidationResult result = await _validator.ValidateAsync(ValidRequest(), CancellationToken.None);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_ShouldFail_WhenSetupTokenIsEmpty(string setupToken)
    {
        VerifyTwoFactorRequest request = ValidRequest() with { SetupToken = setupToken };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(VerifyTwoFactorRequest.SetupToken), result.Errors.Select(e => e.PropertyName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_ShouldFail_WhenCodeIsEmpty(string code)
    {
        VerifyTwoFactorRequest request = ValidRequest() with { Code = code };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(VerifyTwoFactorRequest.Code), result.Errors.Select(e => e.PropertyName));
    }
}
