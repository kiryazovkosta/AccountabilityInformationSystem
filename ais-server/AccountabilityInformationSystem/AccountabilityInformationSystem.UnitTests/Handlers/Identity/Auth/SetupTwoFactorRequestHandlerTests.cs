using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.SetupTwoFactor;
using AccountabilityInformationSystem.UnitTests.Fakes;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace AccountabilityInformationSystem.UnitTests.Handlers.Identity.Auth;

public sealed class SetupTwoFactorRequestHandlerTests
{
    private readonly EphemeralDataProtectionProvider _dataProtectionProvider = new();

    [Fact]
    public async Task Handle_ShouldFail_WhenSetupTokenCannotBeDecrypted()
    {
        // Arrange - protected with a different provider instance, so the handler's provider cannot decrypt it.
        FakeUserManager userManager = new();
        SetupTwoFactorRequestHandler handler = new(userManager, _dataProtectionProvider);
        string tokenFromAnotherProvider = new EphemeralDataProtectionProvider()
            .CreateProtector("TwoFactorSetupToken")
            .ToTimeLimitedDataProtector()
            .Protect("user-1", TimeSpan.FromMinutes(10));

        // Act
        Result<SetupTwoFactorResponse> result = await handler.Handle(
            new SetupTwoFactorRequest { SetupToken = tokenFromAnotherProvider },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        FakeUserManager userManager = new()
        {
            FindByIdAsyncFunc = _ => Task.FromResult<IdentityUser?>(null)
        };
        SetupTwoFactorRequestHandler handler = new(userManager, _dataProtectionProvider);

        // Act
        Result<SetupTwoFactorResponse> result = await handler.Handle(
            new SetupTwoFactorRequest { SetupToken = CreateSetupToken("user-1") },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ResultFailureType.NotFound, result.FailureType);
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenTwoFactorAlreadyEnabled()
    {
        // Arrange
        IdentityUser user = new("user1") { Id = "user-1", TwoFactorEnabled = true };
        FakeUserManager userManager = new()
        {
            FindByIdAsyncFunc = _ => Task.FromResult<IdentityUser?>(user)
        };
        SetupTwoFactorRequestHandler handler = new(userManager, _dataProtectionProvider);

        // Act
        Result<SetupTwoFactorResponse> result = await handler.Handle(
            new SetupTwoFactorRequest { SetupToken = CreateSetupToken(user.Id) },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ResultFailureType.Conflict, result.FailureType);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenAuthenticatorKeyGenerationReturnsEmpty()
    {
        // Arrange
        IdentityUser user = new("user1") { Id = "user-1", TwoFactorEnabled = false };
        FakeUserManager userManager = new()
        {
            FindByIdAsyncFunc = _ => Task.FromResult<IdentityUser?>(user),
            ResetAuthenticatorKeyAsyncFunc = _ => Task.FromResult(IdentityResult.Success),
            GetAuthenticatorKeyAsyncFunc = _ => Task.FromResult<string?>(null)
        };
        SetupTwoFactorRequestHandler handler = new(userManager, _dataProtectionProvider);

        // Act
        Result<SetupTwoFactorResponse> result = await handler.Handle(
            new SetupTwoFactorRequest { SetupToken = CreateSetupToken(user.Id) },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_AndReturnQrCodeAndManualEntryKey()
    {
        // Arrange
        IdentityUser user = new("user1") { Id = "user-1", Email = "user1@example.com", TwoFactorEnabled = false };
        FakeUserManager userManager = new()
        {
            FindByIdAsyncFunc = _ => Task.FromResult<IdentityUser?>(user),
            ResetAuthenticatorKeyAsyncFunc = _ => Task.FromResult(IdentityResult.Success),
            GetAuthenticatorKeyAsyncFunc = _ => Task.FromResult<string?>("ABCDEFGHIJ")
        };
        SetupTwoFactorRequestHandler handler = new(userManager, _dataProtectionProvider);

        // Act
        Result<SetupTwoFactorResponse> result = await handler.Handle(
            new SetupTwoFactorRequest { SetupToken = CreateSetupToken(user.Id) },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ABCDEFGHIJ", result.Value!.ManualEntryKey);
        Assert.StartsWith("data:image/png;base64,", result.Value.QrCodeBase64);
    }

    private string CreateSetupToken(string userId) =>
        _dataProtectionProvider.CreateProtector("TwoFactorSetupToken")
            .ToTimeLimitedDataProtector()
            .Protect(userId, TimeSpan.FromMinutes(10));
}
