namespace AccountabilityInformationSystem.IntegrationTests.Infrastructure;

public abstract class AisIntegrationTestFixture(AisAuthWebApplicationFactory factory)
    : IClassFixture<AisAuthWebApplicationFactory>
{
    public HttpClient CreateClient() => factory.CreateClient();
}
