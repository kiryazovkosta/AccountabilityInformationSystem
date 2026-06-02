namespace AccountabilityInformationSystem.Api.Settings;

public sealed class SeedingOptions
{
    public const string SectionName = "Seeding";
    public string AdminEmail { get; init; } = string.Empty;
    public string AdminUsername { get; init; } = string.Empty;
    public string AdminFirstName { get; init; } = string.Empty;
    public string AdminLastName { get; init; } = string.Empty;
    public string AdminPassword { get; init; } = string.Empty;
}
