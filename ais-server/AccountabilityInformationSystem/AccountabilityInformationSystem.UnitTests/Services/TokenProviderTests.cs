using System.IdentityModel.Tokens.Jwt;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Settings;
using AccountabilityInformationSystem.Api.Shared.Services.Tokenizing;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace AccountabilityInformationSystem.UnitTests.Services;

public sealed class TokenProviderTests
{
    private static readonly JwtAuthOptions DefaultOptions = new()
    {
        Issuer = "test-issuer",
        Audience = "test-audience",
        Key = "super-secret-key-that-is-at-least-64-characters-long-for-hs512!!",
        ExpirationInMinutes = 60,
        RefreshTokenExpirationDays = 7
    };

    [Fact]
    public void Create_AccessToken_ExpiresAtFrozenTimePlusConfiguredMinutes()
    {
        DateTimeOffset frozenTime = new(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        FakeTimeProvider timeProvider = new(frozenTime);
        TokenProvider sut = new(Options.Create(DefaultOptions), timeProvider);

        AccessTokenResponse response = sut.Create(new AccessTokenRequest("user-1", "testuser", []));

        JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);
        DateTime expectedExpiry = frozenTime.UtcDateTime.AddMinutes(DefaultOptions.ExpirationInMinutes);
        Assert.Equal(expectedExpiry, token.ValidTo, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_AccessToken_UsesAdvancedFakeTime()
    {
        DateTimeOffset frozenTime = new(2025, 6, 15, 8, 0, 0, TimeSpan.Zero);
        FakeTimeProvider timeProvider = new(frozenTime);
        timeProvider.Advance(TimeSpan.FromHours(2));
        TokenProvider sut = new(Options.Create(DefaultOptions), timeProvider);

        AccessTokenResponse response = sut.Create(new AccessTokenRequest("user-2", "testuser2", []));

        JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);
        DateTime expectedExpiry = frozenTime.UtcDateTime.AddHours(2).AddMinutes(DefaultOptions.ExpirationInMinutes);
        Assert.Equal(expectedExpiry, token.ValidTo, TimeSpan.FromSeconds(1));
    }
}
