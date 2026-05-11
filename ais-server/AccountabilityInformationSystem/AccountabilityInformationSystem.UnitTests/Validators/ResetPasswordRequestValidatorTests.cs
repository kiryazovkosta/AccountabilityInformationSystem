using AccountabilityInformationSystem.Api.Features.Identity.Auth.ResetPassword;
using FluentValidation.Results;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public sealed class ResetPasswordRequestValidatorTests
{
    private readonly ResetPasswordRequestValidator _validator = new();

    private static ResetPasswordRequest ValidRequest() => new()
    {
        UserId = "usr_abc123",
        Code = "dGVzdC10b2tlbg==",
        NewPassword = "NewP@ss123",
        ConfirmPassword = "NewP@ss123"
    };

    [Fact]
    public async Task Validate_ShouldSuccess_WhenAllFieldsAreValid()
    {
        ValidationResult result = await _validator.ValidateAsync(ValidRequest(), CancellationToken.None);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUserIdIsEmpty()
    {
        ResetPasswordRequest request = ValidRequest() with { UserId = "" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(ResetPasswordRequest.UserId), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenCodeIsEmpty()
    {
        ResetPasswordRequest request = ValidRequest() with { Code = "" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(ResetPasswordRequest.Code), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenNewPasswordIsEmpty()
    {
        ResetPasswordRequest request = ValidRequest() with { NewPassword = "", ConfirmPassword = "" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(ResetPasswordRequest.NewPassword), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenPasswordsDoNotMatch()
    {
        ResetPasswordRequest request = ValidRequest() with { ConfirmPassword = "Different@1" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(ResetPasswordRequest.ConfirmPassword), result.Errors.Select(e => e.PropertyName));
    }
}
