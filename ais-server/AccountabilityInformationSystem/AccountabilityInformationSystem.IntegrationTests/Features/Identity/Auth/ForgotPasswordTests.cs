using System.Net;
using System.Net.Http.Json;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.ForgotPassword;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Identity.Auth;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class ForgotPasswordTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ForgotPassword_WithExistingUsername_Returns204()
    {
        // Arrange — register a user first
        RegisterUserRequest registerRequest = new()
        {
            Username = "forgotpwduser",
            Email = "forgotpwduser@example.com",
            FirstName = "Forgot",
            LastName = "User",
            Password = "Test@1234",
            ConfirmPassword = "Test@1234",
            Enable2Fa = false
        };
        await _client.PostAsJsonAsync(Routes.Auth.Register, registerRequest, TestContext.Current.CancellationToken);

        ForgotPasswordRequest request = new() { Username = "forgotpwduser" };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            Routes.Auth.ForgotPassword,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ForgotPassword_WithNonExistingUsername_Returns204()
    {
        // Non-existing username must still return 204 to prevent user enumeration
        ForgotPasswordRequest request = new() { Username = "userThatDoesNotExist99" };

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            Routes.Auth.ForgotPassword,
            request,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ForgotPassword_WithEmptyUsername_Returns400()
    {
        ForgotPasswordRequest request = new() { Username = "" };

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            Routes.Auth.ForgotPassword,
            request,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
