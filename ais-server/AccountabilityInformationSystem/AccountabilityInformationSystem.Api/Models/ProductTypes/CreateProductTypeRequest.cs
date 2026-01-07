namespace AccountabilityInformationSystem.Api.Models.ProductTypes;

public sealed record CreateProductTypeRequest
{
    public required string Name { get; init; }
    public required string FullName { get; init; }
}
