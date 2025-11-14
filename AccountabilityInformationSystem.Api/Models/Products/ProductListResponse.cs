namespace AccountabilityInformationSystem.Api.Models.Products;

public sealed record ProductListResponse
{
    public string Id { get; init; }
    public string Code { get; init; }
    public string FullName { get; init; }
}
