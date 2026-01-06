namespace AccountabilityInformationSystem.Api.Settings;

public sealed class CorsOptions
{
    public const string PolicyName = "AccountabilityInformationSystemCorsPolicy";
    public const string SectionName = "Cors";

    public required string[] AllowedOrigins { get; init; }
}
