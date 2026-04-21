namespace AccountabilityInformationSystem.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class AisIntegrationTestCollection : ICollectionFixture<AisWebApplicationFactory>
{
    public const string Name = "AIS Integration Tests";
}
