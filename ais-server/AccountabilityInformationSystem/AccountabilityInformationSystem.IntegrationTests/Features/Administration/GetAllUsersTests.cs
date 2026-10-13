using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Administration;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class GetAllUsersTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    private const string AdminUsername = "adminlistuser";
    private const string Password = "K0st@123!";

    [Fact]
    public async Task GetAllUsers_ShouldReturnRolesAndAccountStatus_OfEachUser()
    {
        // Arrange
        HttpClient adminClient = await CreateAdminClientAsync();
        string lockedUsername = $"locked{Guid.NewGuid():N}";
        await factory.CreateAuthenticatedClientWithout2FaAsync(lockedUsername, Password);
        using (IServiceScope scope = factory.Services.CreateScope())
        {
            UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            IdentityUser lockedUser = (await userManager.FindByNameAsync(lockedUsername))!;
            await userManager.SetLockoutEndDateAsync(lockedUser, DateTimeOffset.UtcNow.AddDays(1));
        }

        // Act
        HttpResponseMessage response = await adminClient.GetAsync(Routes.Admin.Users, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonElement users = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);

        JsonElement admin = FindUser(users, AdminUsername);
        Assert.Equal([Role.Admin, Role.Member], Roles(admin));
        Assert.True(admin.GetProperty("isConfirmed").GetBoolean());
        Assert.False(admin.GetProperty("isLocked").GetBoolean());

        JsonElement locked = FindUser(users, lockedUsername);
        Assert.Equal([Role.Member], Roles(locked));
        Assert.False(locked.GetProperty("isConfirmed").GetBoolean());
        Assert.True(locked.GetProperty("isLocked").GetBoolean());
    }

    private static JsonElement FindUser(JsonElement users, string username) =>
        users.EnumerateArray().Single(user => user.GetProperty("username").GetString() == username);

    private static List<string> Roles(JsonElement user) =>
        [.. user.GetProperty("roles").EnumerateArray().Select(role => role.GetString()!).Order()];

    private async Task<HttpClient> CreateAdminClientAsync()
    {
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync(AdminUsername, Password);

        using IServiceScope scope = factory.Services.CreateScope();
        UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        IdentityUser identityUser = (await userManager.FindByNameAsync(AdminUsername))!;
        if (!await userManager.IsInRoleAsync(identityUser, Role.Admin))
        {
            await userManager.AddToRoleAsync(identityUser, Role.Admin);
        }

        if (!identityUser.EmailConfirmed)
        {
            string confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            await userManager.ConfirmEmailAsync(identityUser, confirmationToken);
        }

        HttpResponseMessage loginResponse = await client.PostAsJsonAsync(
            Routes.Auth.Login,
            new LoginUserRequest { Username = AdminUsername, Password = Password },
            TestContext.Current.CancellationToken);
        loginResponse.EnsureSuccessStatusCode();

        LoginUserResponse loginResult =
            (await loginResponse.Content.ReadFromJsonAsync<LoginUserResponse>(TestContext.Current.CancellationToken))!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

        return client;
    }
}
