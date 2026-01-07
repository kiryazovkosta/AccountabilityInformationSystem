using AccountabilityInformationSystem.Api.Models.Common;

namespace AccountabilityInformationSystem.Api.Models.Flow.Ikunks;

public sealed record IkunksCollectionResponse : ICollectionResponse<IkunkResponse>
{
    public List<IkunkResponse> Items { get; init; }
}
