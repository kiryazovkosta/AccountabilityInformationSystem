using System.ComponentModel.DataAnnotations;

namespace AccountabilityInformationSystem.Api.Settings;

public sealed class AzureStorageOptions
{
    public const string SectionName = "AzureStorage";
    public string? ConnectionString { get; set; }
    public required string PublicContainer { get; set; }
    public required string PrivateContainer { get; set; }
    public int PrivateSasLifetimeMinutes { get; set; }
    public int PublicSasLifetimeDays { get; set; }
}
