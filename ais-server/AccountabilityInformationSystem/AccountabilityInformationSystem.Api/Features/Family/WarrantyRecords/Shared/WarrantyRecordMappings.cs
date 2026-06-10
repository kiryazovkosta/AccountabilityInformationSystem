using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Create;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Shared;

internal static class WarrantyRecordMappings
{
    public static WarrantyRecord ToEntity(this CreateWarrantyRecordRequest request, string userName)
        => new()
        {
            Id = $"wr_{Guid.CreateVersion7()}",
            WarrantyBrandId = request.WarrantyBrandId,
            Model = request.Model,
            PurchaseDate = request.PurchaseDate,
            Receipt = request.Receipt?.FileName ?? string.Empty,
            FrontImage = request.FrontImage?.FileName ?? string.Empty,
            BackImage = request.BackImage?.FileName ?? string.Empty,
            CreatedBy = userName,
            CreatedAt = DateTime.UtcNow,
        };

    public static WarrantyRecordResponse ToResponse(this WarrantyRecord record)
        => new()
        {
            Id = record.Id,
            WarrantyBrandId = record.WarrantyBrandId,
            Model = record.Model,
            PurchaseDate = record.PurchaseDate,
            Receipt = record.Receipt,
            FrontImage = record.FrontImage,
            BackImage = record.BackImage,
        };
}
