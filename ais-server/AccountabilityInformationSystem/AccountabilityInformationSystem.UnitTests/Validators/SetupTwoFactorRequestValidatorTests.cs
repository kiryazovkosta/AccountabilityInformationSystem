using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.SetupTwoFactor;
using FluentValidation.Results;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public sealed class SetupTwoFactorRequestValidatorTests
{
    private readonly SetupTwoFactorRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenSetupTokenIsValid()
    {
        SetupTwoFactorRequest request = new() { SetupToken = "valid-setup-token" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_ShouldFail_WhenSetupTokenIsEmpty(string setupToken)
    {
        SetupTwoFactorRequest request = new() { SetupToken = setupToken };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(SetupTwoFactorRequest.SetupToken), result.Errors.Select(e => e.PropertyName));
    }
}
