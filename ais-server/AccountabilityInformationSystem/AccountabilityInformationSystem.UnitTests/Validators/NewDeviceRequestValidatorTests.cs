using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.NewDevice;
using FluentValidation.Results;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public sealed class NewDeviceRequestValidatorTests
{
    private readonly NewDeviceRequestValidator _validator = new();

    private static NewDeviceRequest ValidRequest() => new()
    {
        Username = "johndoe",
        Password = "P@ssw0rd",
        RecoveryCode = "ABCD1234EFGH"
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
    public async Task Validate_ShouldFail_WhenUsernameIsEmpty(string username)
    {
        NewDeviceRequest request = ValidRequest() with { Username = username };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(NewDeviceRequest.Username), result.Errors.Select(e => e.PropertyName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_ShouldFail_WhenPasswordIsEmpty(string password)
    {
        NewDeviceRequest request = ValidRequest() with { Password = password };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(NewDeviceRequest.Password), result.Errors.Select(e => e.PropertyName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_ShouldFail_WhenRecoveryCodeIsEmpty(string recoveryCode)
    {
        NewDeviceRequest request = ValidRequest() with { RecoveryCode = recoveryCode };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(NewDeviceRequest.RecoveryCode), result.Errors.Select(e => e.PropertyName));
    }
}
