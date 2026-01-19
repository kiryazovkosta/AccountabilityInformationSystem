namespace AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;

public sealed record UserResponse
{
    public required string Id { get; init; }
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public string? Image { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}
