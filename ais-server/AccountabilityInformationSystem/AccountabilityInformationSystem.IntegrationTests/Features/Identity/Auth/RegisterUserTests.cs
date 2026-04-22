using System.Net;
using System.Net.Http.Json;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Identity.Auth;

[Collection(AisAuthIntegrationTestCollection.Name)]
public sealed class RegisterUserTests(AisAuthWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Register_WithValidRequestWithoutEnable2Fa_Returns201()
    {
        // Arrange
        RegisterUserRequest request = new()
        {
            Username = "testuser",
            Email = "testuser@example.com",
            FirstName = "User",
            MiddleName = "For",
            LastName = "Test",
            Password = "Test@1234",
            ConfirmPassword = "Test@1234",
            Enable2Fa = false
        };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            Routes.Auth.Register, 
            request,
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithInvalidData_Returns400()
    {
        RegisterUserRequest request = new()
        {
            Username = "duplicateuser",
            Email = "duplicate@example.com",
            FirstName = "D",
            LastName = "User",
            Password = "1234",
            ConfirmPassword = "1234",
            Enable2Fa = false
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            Routes.Auth.Register, 
            request,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns400()
    {
        RegisterUserRequest request = new()
        {
            Username = "duplicateuser",
            Email = "duplicate@example.com",
            FirstName = "Duplicate",
            LastName = "User",
            Password = "Test@1234",
            ConfirmPassword = "Test@1234",
            Enable2Fa = false
        };

        await _client.PostAsJsonAsync(
            Routes.Auth.Register, 
            request,
            TestContext.Current.CancellationToken);

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            Routes.Auth.Register, 
            request,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
