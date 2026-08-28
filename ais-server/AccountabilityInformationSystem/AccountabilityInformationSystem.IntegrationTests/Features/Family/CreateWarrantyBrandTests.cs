using System;
using System.Net;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Create;
using AccountabilityInformationSystem.IntegrationTests.Infrastructure;

namespace AccountabilityInformationSystem.IntegrationTests.Features.Family;

[Collection(AisWithAutoEmailConfirmIntegrationTestCollection.Name)]
public sealed class CreateWarrantyBrandTests(AisWithAutoEmailConfirmWebApplicationFactory factory)
{
    [Fact]
    public async Task CreateWarrantyBrand_ShouldSuccess_WithValidParameters()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();

        using StringContent nameContent = new($"Test Brand {Guid.NewGuid():N}");
        using MultipartFormDataContent content = new()
        {
            { nameContent, nameof(CreateWarrantyBrandRequest.Name) }
        };

        // Act
        HttpResponseMessage response = await client.PostAsync(
            Routes.WarrantyBrands.Base,
            content,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateWarrantyBrand_ShouldFail_WithDuplicateName()
    {
        // Arrange
        HttpClient client = await factory.CreateAuthenticatedClientWithout2FaAsync();
        string name = $"Dup Brand {Guid.NewGuid():N}";

        using StringContent firstNameContent = new(name);
        using MultipartFormDataContent firstContent = new()
        {
            { firstNameContent, nameof(CreateWarrantyBrandRequest.Name) }
        };
        await client.PostAsync(Routes.WarrantyBrands.Base, firstContent, TestContext.Current.CancellationToken);

        using StringContent secondNameContent = new(name);
        using MultipartFormDataContent secondContent = new()
        {
            { secondNameContent, nameof(CreateWarrantyBrandRequest.Name) }
        };

        // Act
        HttpResponseMessage response = await client.PostAsync(
            Routes.WarrantyBrands.Base,
            secondContent,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
