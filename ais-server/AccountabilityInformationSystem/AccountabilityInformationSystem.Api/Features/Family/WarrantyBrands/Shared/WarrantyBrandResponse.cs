namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Shared;

public sealed record WarrantyBrandResponse
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Logo { get; init; }
}
