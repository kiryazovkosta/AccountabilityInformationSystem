using AccountabilityInformationSystem.Api.Features.Identity.Auth.ChangePassword;
using FluentValidation.Results;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public sealed class ChangePasswordRequestValidatorTests
{
    private readonly ChangePasswordRequestValidator _validator = new();

    private static ChangePasswordRequest ValidRequest() => new(
        OldPassword: "OldP@ss123",
        NewPassword: "NewP@ss456",
        ConfirmPassword: "NewP@ss456",
        Code: null);

    [Fact]
    public async Task Validate_ShouldSuccess_WhenAllFieldsAreValid()
    {
        ValidationResult result = await _validator.ValidateAsync(ValidRequest(), CancellationToken.None);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenOldPasswordIsEmpty()
    {
        ChangePasswordRequest request = ValidRequest() with { OldPassword = "" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(ChangePasswordRequest.OldPassword), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenNewPasswordIsEmpty()
    {
        ChangePasswordRequest request = ValidRequest() with { NewPassword = "", ConfirmPassword = "" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(ChangePasswordRequest.NewPassword), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenConfirmPasswordIsEmpty()
    {
        ChangePasswordRequest request = ValidRequest() with { ConfirmPassword = "" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(ChangePasswordRequest.ConfirmPassword), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenPasswordsDoNotMatch()
    {
        ChangePasswordRequest request = ValidRequest() with { ConfirmPassword = "Different@999" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(ChangePasswordRequest.NewPassword), result.Errors.Select(e => e.PropertyName));
    }
}
