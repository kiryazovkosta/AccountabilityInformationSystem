using AccountabilityInformationSystem.Api.Shared.Models;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;

public sealed record IkunksCollectionResponse : ICollectionResponse<IkunkResponse>
{
    public List<IkunkResponse> Items { get; init; }
}
