using System.Net;
using System.Net.Http.Json;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.ChangePassword;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Identity.Auth;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class ChangePasswordTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    [Fact]
    public async Task ChangePassword_WithValidCredentials_Returns204()
    {
        const string username = "changepwd_valid";
        const string password = "L1st@123!";
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync(username, password);

        ChangePasswordRequest request = new(
            OldPassword: password,
            NewPassword: "NewL1st@456!",
            ConfirmPassword: "NewL1st@456!",
            Code: null);

        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Auth.ChangePassword, request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_AfterSuccess_CanLoginWithNewPassword()
    {
        const string username = "changepwd_relogin";
        const string oldPassword = "L1st@123!";
        const string newPassword = "NewL1st@789!";
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync(username, oldPassword);

        ChangePasswordRequest changeRequest = new(
            OldPassword: oldPassword,
            NewPassword: newPassword,
            ConfirmPassword: newPassword,
            Code: null);

        HttpResponseMessage changeResponse = await client.PostAsJsonAsync(
            Routes.Auth.ChangePassword, changeRequest, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, changeResponse.StatusCode);

        HttpClient anonClient = factory.CreateClient();
        LoginUserRequest loginRequest = new() { Username = username, Password = newPassword };
        HttpResponseMessage loginResponse = await anonClient.PostAsJsonAsync(
            Routes.Auth.Login, loginRequest, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_WithWrongOldPassword_Returns400()
    {
        const string username = "changepwd_wrongold";
        const string password = "K0st@123!";
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync(username, password);

        ChangePasswordRequest request = new(
            OldPassword: "WrongP@ss999!",
            NewPassword: "NewL1st@456!",
            ConfirmPassword: "NewL1st@456!",
            Code: null);

        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Auth.ChangePassword, request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_WithPasswordMismatch_Returns400()
    {
        const string username = "changepwd_mismatch";
        const string password = "L1st@123!";
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync(username, password);

        ChangePasswordRequest request = new(
            OldPassword: password,
            NewPassword: "NewL1st@456!",
            ConfirmPassword: "DifferentP@ss999!",
            Code: null);

        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Auth.ChangePassword, request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_WithMissingOldPassword_Returns400()
    {
        const string username = "changepwd_missingold";
        const string password = "K0st@123!";
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync(username, password);

        ChangePasswordRequest request = new(
            OldPassword: "",
            NewPassword: "NewL1st@456!",
            ConfirmPassword: "NewL1st@456!",
            Code: null);

        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Auth.ChangePassword, request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_Unauthenticated_Returns401()
    {
        HttpClient client = factory.CreateClient();

        ChangePasswordRequest request = new(
            OldPassword: "K0st@123!",
            NewPassword: "NewL1st@456!",
            ConfirmPassword: "NewL1st@456!",
            Code: null);

        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.Auth.ChangePassword, request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
