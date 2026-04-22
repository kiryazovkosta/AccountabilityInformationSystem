namespace AccountabilityInformationSystem.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class AisWithAutoEmailConfirmIntegrationTestCollection : ICollectionFixture<AisWithAutoEmailConfirmWebApplicationFactory>
{
    public const string Name = "AIS Integration Tests";
}
