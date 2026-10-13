using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.NewDevice;
using AccountabilityInformationSystem.UnitTests.Fakes;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace AccountabilityInformationSystem.UnitTests.Handlers.Identity.Auth;

public sealed class NewDeviceRequestHandlerTests
{
    private readonly EphemeralDataProtectionProvider _dataProtectionProvider = new();

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        using FakeUserManager userManager = new()
        {
            FindByNameAsyncFunc = _ => Task.FromResult<IdentityUser?>(null)
        };
        NewDeviceRequestHandler handler = new(userManager, new TwoFactorSetupTokenProtector(_dataProtectionProvider));

        // Act
        Result<LoginUserResponse> result = await handler.Handle(
            new NewDeviceRequest { Username = "missing", Password = "irrelevant", RecoveryCode = "code" },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ResultFailureType.Unauthorized, result.FailureType);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
    {
        // Arrange
        IdentityUser user = new("user1") { Id = "user-1" };
        using FakeUserManager userManager = new()
        {
            FindByNameAsyncFunc = _ => Task.FromResult<IdentityUser?>(user),
            CheckPasswordAsyncFunc = (_, _) => Task.FromResult(false)
        };
        NewDeviceRequestHandler handler = new(userManager, new TwoFactorSetupTokenProtector(_dataProtectionProvider));

        // Act
        Result<LoginUserResponse> result = await handler.Handle(
            new NewDeviceRequest { Username = "user1", Password = "wrong", RecoveryCode = "code" },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ResultFailureType.Unauthorized, result.FailureType);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenTwoFactorIsNotEnabled()
    {
        // Arrange
        IdentityUser user = new("user1") { Id = "user-1", TwoFactorEnabled = false };
        using FakeUserManager userManager = new()
        {
            FindByNameAsyncFunc = _ => Task.FromResult<IdentityUser?>(user),
            CheckPasswordAsyncFunc = (_, _) => Task.FromResult(true)
        };
        NewDeviceRequestHandler handler = new(userManager, new TwoFactorSetupTokenProtector(_dataProtectionProvider));

        // Act
        Result<LoginUserResponse> result = await handler.Handle(
            new NewDeviceRequest { Username = "user1", Password = "correct", RecoveryCode = "code" },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ResultFailureType.Unauthorized, result.FailureType);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenRecoveryCodeIsInvalid()
    {
        // Arrange
        IdentityUser user = new("user1") { Id = "user-1", TwoFactorEnabled = true };
        using FakeUserManager userManager = new()
        {
            FindByNameAsyncFunc = _ => Task.FromResult<IdentityUser?>(user),
            CheckPasswordAsyncFunc = (_, _) => Task.FromResult(true),
            RedeemTwoFactorRecoveryCodeAsyncFunc = (_, _) => Task.FromResult(IdentityResult.Failed())
        };
        NewDeviceRequestHandler handler = new(userManager, new TwoFactorSetupTokenProtector(_dataProtectionProvider));

        // Act
        Result<LoginUserResponse> result = await handler.Handle(
            new NewDeviceRequest { Username = "user1", Password = "correct", RecoveryCode = "used-code" },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ResultFailureType.Unauthorized, result.FailureType);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_AndRequireTwoFactorSetup()
    {
        // Arrange
        IdentityUser user = new("user1") { Id = "user-1", TwoFactorEnabled = true };
        using FakeUserManager userManager = new()
        {
            FindByNameAsyncFunc = _ => Task.FromResult<IdentityUser?>(user),
            CheckPasswordAsyncFunc = (_, _) => Task.FromResult(true),
            RedeemTwoFactorRecoveryCodeAsyncFunc = (_, _) => Task.FromResult(IdentityResult.Success),
            SetTwoFactorEnabledAsyncFunc = (_, _) => Task.FromResult(IdentityResult.Success)
        };
        NewDeviceRequestHandler handler = new(userManager, new TwoFactorSetupTokenProtector(_dataProtectionProvider));

        // Act
        Result<LoginUserResponse> result = await handler.Handle(
            new NewDeviceRequest { Username = "user1", Password = "correct", RecoveryCode = "valid-code" },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ResultSuccessType.Accepted, result.SuccessType);
        Assert.True(result.Value!.RequiresTwoFactorSetup);
        Assert.False(string.IsNullOrEmpty(result.Value.SetupToken));
    }
}
