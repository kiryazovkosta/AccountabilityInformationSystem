namespace AccountabilityInformationSystem.Api.Shared;

// Registers the services owned by one feature; implementations are discovered by AddFeatureModules.
public interface IFeatureModule
{
    void Register(IServiceCollection services);
}
