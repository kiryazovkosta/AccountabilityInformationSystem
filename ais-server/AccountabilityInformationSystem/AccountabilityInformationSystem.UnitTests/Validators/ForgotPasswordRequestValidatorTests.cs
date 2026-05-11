using AccountabilityInformationSystem.Api.Features.Identity.Auth.ForgotPassword;
using FluentValidation.Results;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public sealed class ForgotPasswordRequestValidatorTests
{
    private readonly ForgotPasswordRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenUsernameIsValid()
    {
        ForgotPasswordRequest request = new() { Username = "johndoe" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUsernameIsEmpty()
    {
        ForgotPasswordRequest request = new() { Username = "" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(ForgotPasswordRequest.Username), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUsernameIsWhitespace()
    {
        ForgotPasswordRequest request = new() { Username = "   " };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(ForgotPasswordRequest.Username), result.Errors.Select(e => e.PropertyName));
    }
}
