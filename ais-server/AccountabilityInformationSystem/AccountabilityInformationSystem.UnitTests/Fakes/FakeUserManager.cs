using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace AccountabilityInformationSystem.UnitTests.Fakes;

// Hand-rolled fake for UserManager<IdentityUser> - the solution has no mocking library (no Moq/NSubstitute)
// and no EF InMemory/Sqlite provider, so this subclasses the real UserManager and overrides only the
// virtual members the handlers under test actually call, mirroring the existing NullEmailSender/FakeFileStorage
// hand-written-fake convention one layer deeper. Every overridden member defaults to throwing so a test that
// forgets to configure a branch it hits fails loudly instead of silently returning a default.
public sealed class FakeUserManager()
    : UserManager<IdentityUser>(
        new NotSupportedUserStore(),
        Microsoft.Extensions.Options.Options.Create(new IdentityOptions()),
        new PasswordHasher<IdentityUser>(),
        [],
        [],
        new UpperInvariantLookupNormalizer(),
        new IdentityErrorDescriber(),
        new ServiceCollection().BuildServiceProvider(),
        NullLogger<UserManager<IdentityUser>>.Instance)
{
    public Func<string, Task<IdentityUser?>>? FindByIdAsyncFunc { get; set; }
    public Func<string, Task<IdentityUser?>>? FindByNameAsyncFunc { get; set; }
    public Func<IdentityUser, string, Task<bool>>? CheckPasswordAsyncFunc { get; set; }
    public Func<IdentityUser, Task<IdentityResult>>? ResetAuthenticatorKeyAsyncFunc { get; set; }
    public Func<IdentityUser, Task<string?>>? GetAuthenticatorKeyAsyncFunc { get; set; }
    public Func<IdentityUser, string, string, Task<bool>>? VerifyTwoFactorTokenAsyncFunc { get; set; }
    public Func<IdentityUser, bool, Task<IdentityResult>>? SetTwoFactorEnabledAsyncFunc { get; set; }
    public Func<IdentityUser, int, Task<IEnumerable<string>?>>? GenerateNewTwoFactorRecoveryCodesAsyncFunc { get; set; }
    public Func<IdentityUser, string, Task<IdentityResult>>? RedeemTwoFactorRecoveryCodeAsyncFunc { get; set; }

    public override Task<IdentityUser?> FindByIdAsync(string userId) =>
        Invoke(FindByIdAsyncFunc, nameof(FindByIdAsync))(userId);

    public override Task<IdentityUser?> FindByNameAsync(string userName) =>
        Invoke(FindByNameAsyncFunc, nameof(FindByNameAsync))(userName);

    public override Task<bool> CheckPasswordAsync(IdentityUser user, string password) =>
        Invoke(CheckPasswordAsyncFunc, nameof(CheckPasswordAsync))(user, password);

    public override Task<IdentityResult> ResetAuthenticatorKeyAsync(IdentityUser user) =>
        Invoke(ResetAuthenticatorKeyAsyncFunc, nameof(ResetAuthenticatorKeyAsync))(user);

    public override Task<string?> GetAuthenticatorKeyAsync(IdentityUser user) =>
        Invoke(GetAuthenticatorKeyAsyncFunc, nameof(GetAuthenticatorKeyAsync))(user);

    public override Task<bool> VerifyTwoFactorTokenAsync(IdentityUser user, string tokenProvider, string token) =>
        Invoke(VerifyTwoFactorTokenAsyncFunc, nameof(VerifyTwoFactorTokenAsync))(user, tokenProvider, token);

    public override Task<IdentityResult> SetTwoFactorEnabledAsync(IdentityUser user, bool enabled) =>
        Invoke(SetTwoFactorEnabledAsyncFunc, nameof(SetTwoFactorEnabledAsync))(user, enabled);

    public override Task<IEnumerable<string>?> GenerateNewTwoFactorRecoveryCodesAsync(IdentityUser user, int number) =>
        Invoke(GenerateNewTwoFactorRecoveryCodesAsyncFunc, nameof(GenerateNewTwoFactorRecoveryCodesAsync))(user, number);

    public override Task<IdentityResult> RedeemTwoFactorRecoveryCodeAsync(IdentityUser user, string code) =>
        Invoke(RedeemTwoFactorRecoveryCodeAsyncFunc, nameof(RedeemTwoFactorRecoveryCodeAsync))(user, code);

    private static T Invoke<T>(T? func, string memberName) where T : class =>
        func ?? throw new InvalidOperationException($"{memberName} was called but not configured on {nameof(FakeUserManager)}.");

    private sealed class NotSupportedUserStore : IUserStore<IdentityUser>
    {
        public void Dispose()
        {
        }

        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }
}
