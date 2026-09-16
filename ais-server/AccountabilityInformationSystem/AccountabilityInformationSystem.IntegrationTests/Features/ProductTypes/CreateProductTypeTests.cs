using System;
using System.Net;
using System.Net.Http.Json;
using AccountabilityInformationSystem.Api.Features.ProductTypes.Create;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;

namespace AccountabilityInformationSystem.IntegrationTests.Features.ProductTypes;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class CreateProductTypeTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    [Fact]
    public async Task CreateProductType_ShouldSucceed_WithValidParameters()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        CreateProductTypeRequest request = new()
        {
            Name = $"PT-{Guid.NewGuid():N}",
            FullName = "Product Type for create tests"
        };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.ProductTypes.Base,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateProductType_ShouldFail_WithInvalidParameters()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        CreateProductTypeRequest request = new()
        {
            Name = string.Empty,
            FullName = "Product Type for create tests"
        };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.ProductTypes.Base,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProductType_ShouldFail_WithDuplicateName()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        CreateProductTypeRequest request = new()
        {
            Name = $"PT-DUP-{Guid.NewGuid():N}",
            FullName = "Product Type for create tests"
        };

        await client.PostAsJsonAsync(Routes.ProductTypes.Base, request, TestContext.Current.CancellationToken);

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            Routes.ProductTypes.Base,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
