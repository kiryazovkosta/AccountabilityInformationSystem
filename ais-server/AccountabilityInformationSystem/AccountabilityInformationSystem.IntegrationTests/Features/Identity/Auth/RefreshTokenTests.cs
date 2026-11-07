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
public sealed class RefreshTokenTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    private const string Password = "K0st@123!";

    [Fact]
    public async Task Refresh_ShouldReturn204AndRotateCookies_WhenRefreshTokenIsValid()
    {
        // Arrange
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });
        HttpResponseMessage loginResponse = await RegisterAndLoginAsync(client);
        string refreshToken = loginResponse.GetCookieValue("refreshToken");

        // Act
        HttpResponseMessage response = await SendRefreshAsync(client, refreshToken);

        // Assert
        string body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(body);
        Assert.DoesNotMatch("(?i)accessToken|refreshToken", body);
        Assert.False(string.IsNullOrEmpty(response.GetCookieValue("accessToken")));
        Assert.NotEqual(refreshToken, response.GetCookieValue("refreshToken"));
    }

    [Fact]
    public async Task Refresh_ShouldReturn401_WhenRefreshTokenCookieIsMissing()
    {
        // Arrange
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });

        // Act
        HttpResponseMessage response = await client.PostAsync(
            Routes.Auth.Refresh,
            null,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_ShouldReturn401_WhenRefreshTokenIsUnknown()
    {
        // Arrange
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });

        // Act
        HttpResponseMessage response = await SendRefreshAsync(client, "unknown-refresh-token");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldNotReturnTokensInBody_AndShouldSetCookies()
    {
        // Arrange
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });

        // Act
        HttpResponseMessage loginResponse = await RegisterAndLoginAsync(client);

        // Assert
        LoginUserResponse? body = await loginResponse.Content.ReadFromJsonAsync<LoginUserResponse>(
            TestContext.Current.CancellationToken);
        Assert.NotNull(body);
        Assert.Null(body.AccessToken);
        Assert.Null(body.RefreshToken);
        Assert.False(string.IsNullOrEmpty(loginResponse.GetCookieValue("accessToken")));
        Assert.False(string.IsNullOrEmpty(loginResponse.GetCookieValue("refreshToken")));
    }

    [Fact]
    public async Task Login_ShouldStoreOnlyHashOfRefreshToken()
    {
        // Arrange
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });

        // Act
        HttpResponseMessage loginResponse = await RegisterAndLoginAsync(client);
        string refreshToken = loginResponse.GetCookieValue("refreshToken");

        // Assert
        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationIdentityDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        bool plaintextStored = await dbContext.RefreshTokens
            .AnyAsync(rt => rt.Token == refreshToken, TestContext.Current.CancellationToken);
        string expectedHash = RefreshTokenHasher.Hash(refreshToken);
        bool hashStored = await dbContext.RefreshTokens
            .AnyAsync(rt => rt.Token == expectedHash, TestContext.Current.CancellationToken);

        Assert.False(plaintextStored);
        Assert.True(hashStored);
    }

    [Fact]
    public async Task Refresh_ShouldRejectOldToken_AfterRotation()
    {
        // Arrange
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });
        HttpResponseMessage loginResponse = await RegisterAndLoginAsync(client);
        string oldRefreshToken = loginResponse.GetCookieValue("refreshToken");
        HttpResponseMessage firstRefresh = await SendRefreshAsync(client, oldRefreshToken);
        Assert.Equal(HttpStatusCode.NoContent, firstRefresh.StatusCode);

        // Act
        HttpResponseMessage response = await SendRefreshAsync(client, oldRefreshToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static async Task<HttpResponseMessage> SendRefreshAsync(HttpClient client, string refreshToken)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, Routes.Auth.Refresh);
        request.Headers.Add("Cookie", $"refreshToken={refreshToken}");

        return await client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    private static async Task<HttpResponseMessage> RegisterAndLoginAsync(HttpClient client)
    {
        string username = $"refresh_{Guid.NewGuid():N}"[..20];

        HttpResponseMessage registerResponse = await client.PostAsJsonAsync(
            Routes.Auth.Register,
            new RegisterUserRequest
            {
                Username = username,
                Email = $"{username}@example.com",
                FirstName = "Refresh",
                LastName = "Token",
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
