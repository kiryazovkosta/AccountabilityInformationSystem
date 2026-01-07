namespace AccountabilityInformationSystem.ConsoleSeeder.DTOs;

internal sealed class LoginResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}