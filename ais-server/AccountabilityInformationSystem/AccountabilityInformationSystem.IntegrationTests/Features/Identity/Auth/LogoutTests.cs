using System.Net;
using System.Net.Http.Json;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.Tokenizing;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Identity.Auth;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class LogoutTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    private const string Password = "K0st@123!";

    private static readonly string[] AuthCookies = ["accessToken", "refreshToken", "XSRF-TOKEN", ".AspNetCore.Antiforgery.AIS"];

    [Fact]
    public async Task Logout_ShouldReturn204ClearCookiesAndRevokeRefreshToken_WhenSessionIsValid()
    {
        // Arrange
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });
        HttpResponseMessage loginResponse = await RegisterAndLoginAsync(client);
        string refreshToken = loginResponse.GetCookieValue("refreshToken");

        // Act
        HttpResponseMessage response = await SendWithCookiesAsync(client, HttpMethod.Post, Routes.Auth.Logout, loginResponse.ToCookieHeader());

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        AssertAuthCookiesCleared(response);
        Assert.False(await RefreshTokenExistsAsync(refreshToken));

        HttpResponseMessage refreshResponse = await SendWithCookiesAsync(
            client, HttpMethod.Post, Routes.Auth.Refresh, $"refreshToken={Uri.EscapeDataString(refreshToken)}");
        Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
    }

    [Fact]
    public async Task Logout_ShouldReturn204AndClearCookies_WhenRefreshTokenIsUnknown()
    {
        // Arrange
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });
        HttpResponseMessage loginResponse = await RegisterAndLoginAsync(client);
        string accessToken = loginResponse.GetCookieValue("accessToken");

        // Act
        HttpResponseMessage response = await SendWithCookiesAsync(
            client, HttpMethod.Post, Routes.Auth.Logout, $"accessToken={accessToken}; refreshToken=unknown-refresh-token");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        AssertAuthCookiesCleared(response);
    }

    [Fact]
    public async Task Logout_ShouldRevokeRefreshToken_WhenAccessTokenCookieHasExpired()
    {
        // Arrange
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });
        HttpResponseMessage loginResponse = await RegisterAndLoginAsync(client);
        string refreshToken = loginResponse.GetCookieValue("refreshToken");

        // Act
        HttpResponseMessage response = await SendWithCookiesAsync(
            client, HttpMethod.Post, Routes.Auth.Logout, $"refreshToken={Uri.EscapeDataString(refreshToken)}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        AssertAuthCookiesCleared(response);
        Assert.False(await RefreshTokenExistsAsync(refreshToken));
    }

    [Fact]
    public async Task Logout_ShouldReturn204AndClearCookies_WhenNoCookiesAreSent()
    {
        // Arrange
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });

        // Act
        HttpResponseMessage response = await client.PostAsync(Routes.Auth.Logout, null, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        AssertAuthCookiesCleared(response);
    }

    private static void AssertAuthCookiesCleared(HttpResponseMessage response)
    {
        foreach (string cookie in AuthCookies)
        {
            Assert.True(response.IsCookieCleared(cookie), $"Expected the '{cookie}' cookie to be cleared.");
        }
    }

    private async Task<bool> RefreshTokenExistsAsync(string refreshToken)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationIdentityDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        string tokenHash = RefreshTokenHasher.Hash(refreshToken);
        return await dbContext.RefreshTokens.AnyAsync(rt => rt.Token == tokenHash, TestContext.Current.CancellationToken);
    }

    private static async Task<HttpResponseMessage> SendWithCookiesAsync(
        HttpClient client,
        HttpMethod method,
        string route,
        string cookieHeader)
    {
        using HttpRequestMessage request = new(method, route);
        request.Headers.Add("Cookie", cookieHeader);

        return await client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    private static async Task<HttpResponseMessage> RegisterAndLoginAsync(HttpClient client)
    {
        string username = $"logout_{Guid.NewGuid():N}"[..20];

        HttpResponseMessage registerResponse = await client.PostAsJsonAsync(
            Routes.Auth.Register,
            new RegisterUserRequest
            {
                Username = username,
                Email = $"{username}@example.com",
                FirstName = "Logout",
                LastName = "User",
                Password = Password,
                ConfirmPassword = Password,
                Enable2Fa = false
            },
            TestContext.Current.CancellationToken);
        registerResponse.EnsureSuccessStatusCode();

        HttpResponseMessage loginResponse = await client.PostAsJsonAsync(
            Routes.Auth.Login,
            new LoginUserRequest { Username = username, Password = Password },
            TestContext.Current.CancellationToken);
        loginResponse.EnsureSuccessStatusCode();

        return loginResponse;
    }
}
