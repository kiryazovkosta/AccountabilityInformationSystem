using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation.Results;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public sealed class RegisterUserRequestValidatorTests
{
    private readonly RegisterUserRequestValidator _validator = new();

    private static RegisterUserRequest ValidRequest() => new()
    {
        Username = "johndoe",
        Email = "john@example.com",
        FirstName = "John",
        LastName = "Doe",
        Password = "P@ssw0rd",
        ConfirmPassword = "P@ssw0rd",
        Enable2Fa = false
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
        RegisterUserRequest request = ValidRequest() with { Username = username };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(RegisterUserRequest.Username), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUsernameExceedsMaxLength()
    {
        RegisterUserRequest request = ValidRequest() with { Username = new string('a', EntitiesConstants.UsernameMaxLength + 1) };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(RegisterUserRequest.Username), result.Errors.Select(e => e.PropertyName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_ShouldFail_WhenEmailIsEmpty(string email)
    {
        RegisterUserRequest request = ValidRequest() with { Email = email };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(RegisterUserRequest.Email), result.Errors.Select(e => e.PropertyName));
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("@nodomain.com")]
    [InlineData("no-at-sign")]
    public async Task Validate_ShouldFail_WhenEmailIsInvalidFormat(string email)
    {
        RegisterUserRequest request = ValidRequest() with { Email = email };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(RegisterUserRequest.Email) &&
            e.ErrorMessage == "A valid email address is required.");
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenEmailExceedsMaxLength()
    {
        string email = new string('a', EntitiesConstants.EmailMaxLength) + "@b.com";
        RegisterUserRequest request = ValidRequest() with { Email = email };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterUserRequest.Email) && e.ErrorMessage.Contains("128"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_ShouldFail_WhenFirstNameIsEmpty(string firstName)
    {
        RegisterUserRequest request = ValidRequest() with { FirstName = firstName };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(RegisterUserRequest.FirstName), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenFirstNameExceedsMaxLength()
    {
        RegisterUserRequest request = ValidRequest() with { FirstName = new string('a', EntitiesConstants.FirstNameMaxLength + 1) };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(RegisterUserRequest.FirstName), result.Errors.Select(e => e.PropertyName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_ShouldFail_WhenLastNameIsEmpty(string lastName)
    {
        RegisterUserRequest request = ValidRequest() with { LastName = lastName };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(RegisterUserRequest.LastName), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenLastNameExceedsMaxLength()
    {
        RegisterUserRequest request = ValidRequest() with { LastName = new string('a', EntitiesConstants.LastNameMaxLength + 1) };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(RegisterUserRequest.LastName), result.Errors.Select(e => e.PropertyName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_ShouldFail_WhenPasswordIsEmpty(string password)
    {
        RegisterUserRequest request = ValidRequest() with { Password = password, ConfirmPassword = password };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(RegisterUserRequest.Password), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenPasswordIsTooShort()
    {
        RegisterUserRequest request = ValidRequest() with { Password = "abc", ConfirmPassword = "abc" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(RegisterUserRequest.Password), result.Errors.Select(e => e.PropertyName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_ShouldFail_WhenConfirmPasswordIsEmpty(string confirmPassword)
    {
        RegisterUserRequest request = ValidRequest() with { ConfirmPassword = confirmPassword };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(RegisterUserRequest.ConfirmPassword), result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenPasswordsDoNotMatch()
    {
        RegisterUserRequest request = ValidRequest() with { ConfirmPassword = "Different@1" };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(RegisterUserRequest.ConfirmPassword), result.Errors.Select(e => e.PropertyName));
    }
}
