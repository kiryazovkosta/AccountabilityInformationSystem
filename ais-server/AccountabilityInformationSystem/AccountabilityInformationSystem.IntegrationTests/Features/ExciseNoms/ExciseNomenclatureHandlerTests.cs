using System;
using System.Net;
using System.Net.Http.Json;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;

namespace AccountabilityInformationSystem.IntegrationTests.Features.ExciseNoms;

// Covers the generic Create/Update/ToggleStatus handlers shared by ApCodes, BrandNames and CnCodes
// (CreateExciseNomenclatureRequestHandler<TEntity,TCreateRequest>, UpdateExciseNomenclatureRequestHandler<TEntity>,
// ToggleExciseNomenclatureStatusRequestHandler<TEntity>) by exercising all three concrete routes with one
// parameterized test body, since the branching logic is identical across the three entities.
[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class ExciseNomenclatureHandlerTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    public static TheoryData<string, Func<string>> EntityRoutes => new()
    {
        { Routes.ExciseNoms.ApCodes.Base, NextApCodeCode },
        { Routes.ExciseNoms.BrandNames.Base, NextBrandNameCode },
        { Routes.ExciseNoms.CnCodes.Base, NextCnCodeCode }
    };

    [Theory]
    [MemberData(nameof(EntityRoutes))]
    public async Task Create_ShouldSucceed_WithValidParameters(string baseRoute, Func<string> nextCode)
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            baseRoute,
            NewRequestBody(nextCode()),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(EntityRoutes))]
    public async Task Create_ShouldFail_WithDuplicateCode(string baseRoute, Func<string> nextCode)
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        var body = NewRequestBody(nextCode());
        await client.PostAsJsonAsync(baseRoute, body, TestContext.Current.CancellationToken);

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            baseRoute,
            body,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(EntityRoutes))]
    public async Task Update_ShouldFail_WhenEntityDoesNotExist(string baseRoute, Func<string> nextCode)
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string nonExistingId = $"nx_{Guid.NewGuid():N}";

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync(
            $"{baseRoute}/{nonExistingId}",
            NewRequestBody(nextCode()),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(EntityRoutes))]
    public async Task Update_ShouldFail_WithDuplicateCode(string baseRoute, Func<string> nextCode)
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string firstCode = nextCode();
        string secondCode = nextCode();

        await CreateAsync(client, baseRoute, firstCode);
        string secondId = await CreateAsync(client, baseRoute, secondCode);

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync(
            $"{baseRoute}/{secondId}",
            NewRequestBody(firstCode),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(EntityRoutes))]
    public async Task ToggleStatus_ShouldSucceed_AndFlipIsUsed(string baseRoute, Func<string> nextCode)
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string id = await CreateAsync(client, baseRoute, nextCode());

        // Act
        HttpResponseMessage response = await client.PatchAsync(
            $"{baseRoute}/{id}/toggle-status",
            content: null,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(EntityRoutes))]
    public async Task ToggleStatus_ShouldFail_WhenEntityDoesNotExist(string baseRoute, Func<string> nextCode)
    {
        _ = nextCode;

        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string nonExistingId = $"nx_{Guid.NewGuid():N}";

        // Act
        HttpResponseMessage response = await client.PatchAsync(
            $"{baseRoute}/{nonExistingId}/toggle-status",
            content: null,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static async Task<string> CreateAsync(HttpClient client, string baseRoute, string code)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            baseRoute,
            NewRequestBody(code),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        ExciseNomenclatureResponse created =
            (await response.Content.ReadFromJsonAsync<ExciseNomenclatureResponse>(TestContext.Current.CancellationToken))!;
        return created.Id;
    }

    private static object NewRequestBody(string code) => new
    {
        Code = code,
        DescriptionBg = "Тестово описание",
        DescriptionEn = "Test description",
        IsUsed = false
    };

    private static string NextApCodeCode() => $"{(char)('A' + Random.Shared.Next(0, 26))}{Random.Shared.Next(0, 999):D3}";

    private static string NextBrandNameCode() => $"{(char)('A' + Random.Shared.Next(0, 26))}{Random.Shared.Next(0, 99999):D5}";

    private static string NextCnCodeCode() => $"{Random.Shared.Next(1, 99999999):D8}";
}
