using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Administration.Users.GetAll;

public sealed record UsersListResponse : IMapFrom<User>
{
    public string Id { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string LastName { get; init; }
    public string? Image { get; init; }
    public bool? Enable2Fa { get; init; }
    public string? IdentityId { get; init; }
    public bool IsConfirmed { get; init; }
    public bool IsLocked { get; init; }
    public IList<string> Roles { get; init; }
}
