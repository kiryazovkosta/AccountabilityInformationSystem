using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.VerifyTwoFactor;
using AccountabilityInformationSystem.UnitTests.Fakes;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace AccountabilityInformationSystem.UnitTests.Handlers.Identity.Auth;

public sealed class VerifyTwoFactorRequestHandlerTests
{
    private readonly EphemeralDataProtectionProvider _dataProtectionProvider = new();

    [Fact]
    public async Task Handle_ShouldFail_WhenSetupTokenCannotBeDecrypted()
    {
        // Arrange
        FakeUserManager userManager = new();
        VerifyTwoFactorRequestHandler handler = new(userManager, _dataProtectionProvider);
        string tokenFromAnotherProvider = new EphemeralDataProtectionProvider()
            .CreateProtector("TwoFactorSetupToken")
            .ToTimeLimitedDataProtector()
            .Protect("user-1", TimeSpan.FromMinutes(10));

        // Act
        Result<VerifyTwoFactorResponse> result = await handler.Handle(
            new VerifyTwoFactorRequest { SetupToken = tokenFromAnotherProvider, Code = "123456" },
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
        VerifyTwoFactorRequestHandler handler = new(userManager, _dataProtectionProvider);

        // Act
        Result<VerifyTwoFactorResponse> result = await handler.Handle(
            new VerifyTwoFactorRequest { SetupToken = CreateSetupToken("user-1"), Code = "123456" },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ResultFailureType.NotFound, result.FailureType);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCodeIsInvalid()
    {
        // Arrange
        IdentityUser user = new("user1") { Id = "user-1" };
        FakeUserManager userManager = new()
        {
            FindByIdAsyncFunc = _ => Task.FromResult<IdentityUser?>(user),
            VerifyTwoFactorTokenAsyncFunc = (_, _, _) => Task.FromResult(false)
        };
        VerifyTwoFactorRequestHandler handler = new(userManager, _dataProtectionProvider);

        // Act
        Result<VerifyTwoFactorResponse> result = await handler.Handle(
            new VerifyTwoFactorRequest { SetupToken = CreateSetupToken(user.Id), Code = "000000" },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRecoveryCodeGenerationReturnsEmpty()
    {
        // Arrange
        IdentityUser user = new("user1") { Id = "user-1" };
        FakeUserManager userManager = new()
        {
            FindByIdAsyncFunc = _ => Task.FromResult<IdentityUser?>(user),
            VerifyTwoFactorTokenAsyncFunc = (_, _, _) => Task.FromResult(true),
            SetTwoFactorEnabledAsyncFunc = (_, _) => Task.FromResult(IdentityResult.Success),
            GenerateNewTwoFactorRecoveryCodesAsyncFunc = (_, _) => Task.FromResult<IEnumerable<string>?>([])
        };
        VerifyTwoFactorRequestHandler handler = new(userManager, _dataProtectionProvider);

        // Act
        Result<VerifyTwoFactorResponse> result = await handler.Handle(
            new VerifyTwoFactorRequest { SetupToken = CreateSetupToken(user.Id), Code = "123456" },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_AndReturnRecoveryCodes()
    {
        // Arrange
        IdentityUser user = new("user1") { Id = "user-1" };
        string[] recoveryCodes = ["code-1", "code-2"];
        FakeUserManager userManager = new()
        {
            FindByIdAsyncFunc = _ => Task.FromResult<IdentityUser?>(user),
            VerifyTwoFactorTokenAsyncFunc = (_, _, _) => Task.FromResult(true),
            SetTwoFactorEnabledAsyncFunc = (_, _) => Task.FromResult(IdentityResult.Success),
            GenerateNewTwoFactorRecoveryCodesAsyncFunc = (_, _) => Task.FromResult<IEnumerable<string>?>(recoveryCodes)
        };
        VerifyTwoFactorRequestHandler handler = new(userManager, _dataProtectionProvider);

        // Act
        Result<VerifyTwoFactorResponse> result = await handler.Handle(
            new VerifyTwoFactorRequest { SetupToken = CreateSetupToken(user.Id), Code = "123456" },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(recoveryCodes, result.Value!.RecoveryCodes);
    }

    private string CreateSetupToken(string userId) =>
        _dataProtectionProvider.CreateProtector("TwoFactorSetupToken")
            .ToTimeLimitedDataProtector()
            .Protect(userId, TimeSpan.FromMinutes(10));
}
