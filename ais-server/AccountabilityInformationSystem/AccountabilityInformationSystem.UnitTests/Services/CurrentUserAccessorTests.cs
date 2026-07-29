using System.Security.Claims;
using AccountabilityInformationSystem.Api.Shared.Services.CurrentUserAccessing;
using Microsoft.AspNetCore.Http;

namespace AccountabilityInformationSystem.UnitTests.Services;

public sealed class CurrentUserAccessorTests
{
    [Fact]
    public void GetCurrentUser_ShouldReturnIdAndUsername_WhenClaimsArePresent()
    {
        ClaimsIdentity identity = new(
        [
            new Claim(ClaimTypes.NameIdentifier, "user-1"),
            new Claim(ClaimTypes.Name, "testuser")
        ]);
        HttpContextAccessor httpContextAccessor = new()
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
        CurrentUserAccessor sut = new(httpContextAccessor);

        CurrentUser result = sut.GetCurrentUser();

        Assert.Equal("user-1", result.UserId);
        Assert.Equal("testuser", result.UserName);
    }

    [Fact]
    public void GetCurrentUser_ShouldReturnNullValues_WhenHttpContextIsNull()
    {
        HttpContextAccessor httpContextAccessor = new() { HttpContext = null };
        CurrentUserAccessor sut = new(httpContextAccessor);

        CurrentUser result = sut.GetCurrentUser();

        Assert.Null(result.UserId);
        Assert.Null(result.UserName);
    }

    [Fact]
    public void GetCurrentUser_ShouldReturnNullValues_WhenClaimsAreMissing()
    {
        HttpContextAccessor httpContextAccessor = new()
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
        };
        CurrentUserAccessor sut = new(httpContextAccessor);

        CurrentUser result = sut.GetCurrentUser();

        Assert.Null(result.UserId);
        Assert.Null(result.UserName);
    }
}
