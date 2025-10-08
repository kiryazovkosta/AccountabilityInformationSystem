namespace AccountabilityInformationSystem.Api.Models.Flow.Ikunks;

public sealed record IkunksCollectionResponse
{
    public List<IkunkResponse> Items { get; init; }
}
