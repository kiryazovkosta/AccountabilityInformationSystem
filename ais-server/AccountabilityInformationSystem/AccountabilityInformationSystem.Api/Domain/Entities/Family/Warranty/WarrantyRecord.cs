using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

namespace AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;

public sealed class WarrantyRecord : AuditableEntity, IEntity
{
    public string Id { get; set; }
    public string WarrantyBrandId { get; set; }
    public WarrantyBrand WarrantyBrand { get; set; }
    public string Model { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public string? Receipt { get; set; }
    public string? FrontImage { get; set; }
    public string? BackImage { get; set; }
}
