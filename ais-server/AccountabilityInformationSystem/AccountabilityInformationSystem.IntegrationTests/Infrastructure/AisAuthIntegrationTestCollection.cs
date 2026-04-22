namespace AccountabilityInformationSystem.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class AisAuthIntegrationTestCollection : ICollectionFixture<AisAuthWebApplicationFactory>
{
    public const string Name = "AIS Auth Integration Tests";
}
