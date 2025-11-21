namespace AccountabilityInformationSystem.Api.Models.ProductTypes;

public sealed record ProductTypeProductResponse
{
    public string Id { get; init; }
    public string Code { get; init; }
    public string FullName { get; init; }
}
