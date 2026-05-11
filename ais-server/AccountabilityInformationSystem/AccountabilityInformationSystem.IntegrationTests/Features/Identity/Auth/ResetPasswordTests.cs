using System.Net;
using System.Net.Http.Json;
using System.Text;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.ResetPassword;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Identity.Auth;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class ResetPasswordTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task<(string UserId, string Code)> RegisterAndGetResetTokenAsync(string username, string password)
    {
        RegisterUserRequest registerRequest = new()
        {
            Username = username,
            Email = $"{username}@example.com",
            FirstName = "Reset",
            LastName = "User",
            Password = password,
            ConfirmPassword = password,
            Enable2Fa = false
        };
        await _client.PostAsJsonAsync(Routes.Auth.Register, registerRequest, TestContext.Current.CancellationToken);

        using IServiceScope scope = factory.Services.CreateScope();
        UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        IdentityUser identityUser = (await userManager.FindByNameAsync(username))!;
        string rawToken = await userManager.GeneratePasswordResetTokenAsync(identityUser);
        string code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));

        return (identityUser.Id, code);
    }

    [Fact]
    public async Task ResetPassword_WithValidToken_Returns204AndAllowsLoginWithNewPassword()
    {
        // Arrange
        const string username = "resetpwduser";
        const string originalPassword = "Test@1234";
        const string newPassword = "NewTest@5678";

        (string userId, string code) = await RegisterAndGetResetTokenAsync(username, originalPassword);

        ResetPasswordRequest request = new()
        {
            UserId = userId,
            Code = code,
            NewPassword = newPassword,
            ConfirmPassword = newPassword
        };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            Routes.Auth.ResetPassword,
            request,
            TestContext.Current.CancellationToken);

        // Assert — reset succeeded
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Assert — can log in with new password
        LoginUserRequest loginRequest = new() { Username = username, Password = newPassword };
        HttpResponseMessage loginResponse = await _client.PostAsJsonAsync(
            Routes.Auth.Login,
            loginRequest,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WithInvalidToken_Returns400()
    {
        // Arrange — register a real user but supply garbage token
        const string username = "resetpwdinvalidtoken";
        const string password = "Test@1234";

        await _client.PostAsJsonAsync(Routes.Auth.Register, new RegisterUserRequest
        {
            Username = username,
            Email = $"{username}@example.com",
            FirstName = "Reset",
            LastName = "User",
            Password = password,
            ConfirmPassword = password,
            Enable2Fa = false
        }, TestContext.Current.CancellationToken);

        using IServiceScope scope = factory.Services.CreateScope();
        UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        IdentityUser identityUser = (await userManager.FindByNameAsync(username))!;

        string invalidCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("this-is-not-a-valid-token"));

        ResetPasswordRequest request = new()
        {
            UserId = identityUser.Id,
            Code = invalidCode,
            NewPassword = "NewTest@5678",
            ConfirmPassword = "NewTest@5678"
        };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            Routes.Auth.ResetPassword,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WithInvalidUserId_Returns400()
    {
        ResetPasswordRequest request = new()
        {
            UserId = "non-existent-user-id",
            Code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("some-token")),
            NewPassword = "NewTest@5678",
            ConfirmPassword = "NewTest@5678"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            Routes.Auth.ResetPassword,
            request,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WithPasswordMismatch_Returns400()
    {
        ResetPasswordRequest request = new()
        {
            UserId = "some-user-id",
            Code = "someCode",
            NewPassword = "NewTest@5678",
            ConfirmPassword = "Different@9999"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            Routes.Auth.ResetPassword,
            request,
            TestContext.Current.CancellationToken);

        // Validator fires before handler → 400 (UnprocessableEntity from FluentValidation)
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
