namespace AccountabilityInformationSystem.Api.Settings;

public class FrontendOptions
{
    public const string SectionName = "Frontend";
    public string HostName { get; init; }

    public string ConfirmEmail { get; init; }
}
