using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AccountabilityInformationSystem.IntegrationTests.Features.ExciseNoms;

// The excise nomenclature endpoints are marked cacheable with [ResponseCache]. The browser authenticates with the
// accessToken cookie, not an Authorization header, so these tests send the token the same way: the response caching
// middleware never caches requests that carry an Authorization header.
[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class ResponseCachingTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    [Fact]
    public async Task GetAll_ShouldRejectAnonymousRequest_WhenAuthenticatedResponseIsCached()
    {
        // Arrange
        HttpClient cookieClient = await CreateCookieAuthenticatedClientAsync();
        HttpClient anonymousClient = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });
        HttpResponseMessage authenticatedResponse = await cookieClient.GetAsync(
            Routes.ExciseNoms.ApCodes.Base,
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, authenticatedResponse.StatusCode);

        // Act
        HttpResponseMessage response = await anonymousClient.GetAsync(
            Routes.ExciseNoms.ApCodes.Base,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ShouldNotServeCachedResponse_ForDifferentQueryString()
    {
        // Arrange
        HttpClient cookieClient = await CreateCookieAuthenticatedClientAsync();
        await cookieClient.GetAsync($"{Routes.ExciseNoms.CnCodes.Base}?pageSize=1", TestContext.Current.CancellationToken);

        // Act
        HttpResponseMessage response = await cookieClient.GetAsync(
            $"{Routes.ExciseNoms.CnCodes.Base}?pageSize=2",
            TestContext.Current.CancellationToken);

        // Assert
        JsonElement body = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        Assert.Equal(2, body.GetProperty("pageSize").GetInt32());
    }

    private async Task<HttpClient> CreateCookieAuthenticatedClientAsync()
    {
        HttpClient bearerClient = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string accessToken = bearerClient.DefaultRequestHeaders.Authorization!.Parameter!;

        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });
        client.DefaultRequestHeaders.Add("Cookie", $"accessToken={accessToken}");
        return client;
    }
}
