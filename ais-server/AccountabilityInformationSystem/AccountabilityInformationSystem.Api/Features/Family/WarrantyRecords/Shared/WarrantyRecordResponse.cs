namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Shared;

public sealed record WarrantyRecordResponse
{
    public required string Id { get; init; }
    public required string WarrantyBrandId { get; init; }
    public required string Model { get; init; }
    public required DateOnly PurchaseDate { get; init; }
    public string? Receipt { get; init; }
    public string? FrontImage { get; init; }
    public string? BackImage { get; init; }
}
