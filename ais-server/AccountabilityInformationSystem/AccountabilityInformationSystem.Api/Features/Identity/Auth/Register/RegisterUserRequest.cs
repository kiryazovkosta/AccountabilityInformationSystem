using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;
using Mapster;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;

public sealed record RegisterUserRequest : IMapTo<User>, IMapCustom
{
    public string Username { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string LastName { get; init; }
    public string? Image { get; init; }
    public string Password { get; init; }
    public string ConfirmPassword { get; init; }
    public required bool Enable2Fa { get; init; }

    public void CreateMappings(TypeAdapterConfig config) =>
        config.NewConfig<RegisterUserRequest, User>()
            .Map(dest => dest.Id, _ => $"u_{Guid.CreateVersion7()}")
            .Map(dest => dest.CreatedAt, _ => DateTime.UtcNow)
            .Map(dest => dest.CreatedBy, _ => EntitiesConstants.DefaultSystemUser);
}