using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Common;

namespace AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;

public sealed class WarrantyRecord : AuditableEntity, IEntity
{
    public string Id { get; set; }
    public string WarrantyBrandId { get; set; }
    public WarrantyBrand WarrantyBrand { get; set; }
    public string Model { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public string? ReceiptId { get; set; }
    public StorageFile? Receipt { get; set; }
    public string? FrontImageId { get; set; }
    public StorageFile? FrontImage { get; set; }
    public string? BackImageId { get; set; }
    public StorageFile? BackImage { get; set; }
}
