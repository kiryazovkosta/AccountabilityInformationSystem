namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Create;

public sealed record CreateWarrantyRecordRequest
{
    public required string WarrantyBrandId { get; init; }
    public required string Model { get; init; }
    public required DateOnly PurchaseDate { get; init; }
    public IFormFile? Receipt { get; init; }
    public IFormFile? FrontImage { get; init; }
    public IFormFile? BackImage { get; init; }
    public required int Duration { get; init; } 
}
