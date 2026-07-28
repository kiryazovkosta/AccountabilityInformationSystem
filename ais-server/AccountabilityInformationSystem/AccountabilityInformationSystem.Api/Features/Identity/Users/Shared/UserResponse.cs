using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;

public sealed record UserResponse : IMapFrom<User>, IMapCustom
{
    public required string Id { get; init; }
    public string Username { get; init; }
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public string? Image { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];

    public void CreateMappings(Mapster.TypeAdapterConfig config) =>
        config.NewConfig<User, UserResponse>()
            .Map(dest => dest.FullName, src => src.MiddleName != null
                ? $"{src.FirstName} {src.MiddleName} {src.LastName}"
                : $"{src.FirstName} {src.LastName}");
}
