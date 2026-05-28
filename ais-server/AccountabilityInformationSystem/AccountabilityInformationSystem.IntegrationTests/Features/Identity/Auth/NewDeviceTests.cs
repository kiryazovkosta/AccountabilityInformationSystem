using System.Net;
using System.Net.Http.Json;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.NewDevice;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Identity.Auth;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class NewDeviceTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    [Fact]
    public async Task NewDevice_ShouldReturn400_WhenRequestIsEmpty()
    {
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.TwoFactor.NewDevice,
            new { },
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task NewDevice_ShouldReturn401_WhenCredentialsAreInvalid()
    {
        HttpClient client = factory.CreateClient();

        NewDeviceRequest request = new()
        {
            Username = $"newdevice_nonexistent_{Guid.NewGuid():N}",
            Password = "WrongP@ss123!",
            RecoveryCode = "ABCD1234EFGH"
        };

        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.TwoFactor.NewDevice,
            request,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task NewDevice_ShouldReturn401_WhenTwoFactorIsNotEnabled()
    {
        HttpClient client = factory.CreateClient();

        string username = $"newdevice_no2fa_{Guid.NewGuid():N}";
        const string password = "T3st@Pass!";

        HttpResponseMessage registerResponse = await client.PostAsJsonAsync(
            Routes.Auth.Register,
            new RegisterUserRequest
            {
                Username = username,
                Email = $"{username}@example.com",
                FirstName = "New",
                LastName = "Device",
                Password = password,
                ConfirmPassword = password,
                Enable2Fa = false
            },
            TestContext.Current.CancellationToken);
        registerResponse.EnsureSuccessStatusCode();

        NewDeviceRequest request = new()
        {
            Username = username,
            Password = password,
            RecoveryCode = "ABCD1234EFGH"
        };

        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.TwoFactor.NewDevice,
            request,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task NewDevice_ShouldReturn200_WhenRecoveryCodeIsValid()
    {
        HttpClient client = factory.CreateClient();

        string username = $"newdevice_test_{Guid.NewGuid():N}";
        const string password = "T3st@Pass!";

        HttpResponseMessage registerResponse = await client.PostAsJsonAsync(
            Routes.Auth.Register,
            new RegisterUserRequest
            {
                Username = username,
                Email = $"{username}@example.com",
                FirstName = "New",
                LastName = "Device",
                Password = password,
                ConfirmPassword = password,
                Enable2Fa = false
            },
            TestContext.Current.CancellationToken);
        registerResponse.EnsureSuccessStatusCode();

        using IServiceScope scope = factory.Services.CreateScope();
        UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        IdentityUser user = (await userManager.FindByNameAsync(username))!;
        await userManager.ResetAuthenticatorKeyAsync(user);
        IEnumerable<string> codes = (await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10))!;
        await userManager.SetTwoFactorEnabledAsync(user, true);

        NewDeviceRequest request = new()
        {
            Username = username,
            Password = password,
            RecoveryCode = codes.First()
        };

        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.TwoFactor.NewDevice,
            request,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        LoginUserResponse? body = await response.Content.ReadFromJsonAsync<LoginUserResponse>(
            TestContext.Current.CancellationToken);

        Assert.NotNull(body);
        Assert.False(string.IsNullOrEmpty(body.AccessToken));
    }
}
