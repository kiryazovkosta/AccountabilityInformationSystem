using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using FluentValidation.Results;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public sealed class LoginUserRequestValidatorTests
{
    private readonly LoginUserRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenAllFieldsAreValid()
    {
        LoginUserRequest request = new() { Username = "johndoe", Password = "P@ssw0rd" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_ShouldFail_WhenUsernameIsEmpty(string username)
    {
        LoginUserRequest request = new() { Username = username, Password = "P@ssw0rd" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(LoginUserRequest.Username), result.Errors.Select(e => e.PropertyName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_ShouldFail_WhenPasswordIsEmpty(string password)
    {
        LoginUserRequest request = new() { Username = "johndoe", Password = password };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(LoginUserRequest.Password), result.Errors.Select(e => e.PropertyName));
    }
}
