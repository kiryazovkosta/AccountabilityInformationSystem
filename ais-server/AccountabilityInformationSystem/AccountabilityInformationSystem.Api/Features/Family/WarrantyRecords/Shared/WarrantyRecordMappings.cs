using AccountabilityInformationSystem.Api.Domain.Entities.Common;
using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Create;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Shared.Services.FileStoraging;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Shared;

internal static class WarrantyRecordMappings
{
    public static readonly SortMappingDefinition<WarrantyRecordListResponse, WarrantyRecord> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(WarrantyRecordListResponse.Model), nameof(WarrantyRecord.Model)),
            new SortMapping(nameof(WarrantyRecordListResponse.PurchaseDate), nameof(WarrantyRecord.PurchaseDate)),
            new SortMapping(nameof(WarrantyRecordListResponse.WarrantyBrandName), nameof(WarrantyRecord.WarrantyBrand.Name)),
            new SortMapping(nameof(WarrantyRecordListResponse.ReceiptExists), nameof(WarrantyRecord.Receipt)),
            new SortMapping(nameof(WarrantyRecordListResponse.FrontImageExists), nameof(WarrantyRecord.FrontImage)),
            new SortMapping(nameof(WarrantyRecordListResponse.BackImageExists), nameof(WarrantyRecord.BackImage)),
            new SortMapping(nameof(WarrantyRecordListResponse.Duration), nameof(WarrantyRecord.Duration)),
            new SortMapping(nameof(WarrantyRecordListResponse.EndDate), nameof(WarrantyRecord.EndDate)),
            new SortMapping(nameof(WarrantyRecordListResponse.Status), nameof(WarrantyRecord.Status))
        ]
    };

    public static WarrantyRecord ToEntity(this CreateWarrantyRecordRequest request, string userName)
        => new()
        {
            Id = $"wr_{Guid.CreateVersion7()}",
            WarrantyBrandId = request.WarrantyBrandId,
            Model = request.Model,
            PurchaseDate = request.PurchaseDate,
            Duration = request.Duration,
            CreatedBy = userName,
            CreatedAt = DateTime.UtcNow,
        };

    public static WarrantyRecordResponse ToResponse(this WarrantyRecord record)
        => new()
        {
            Id = record.Id,
            WarrantyBrand = new WarrantyBrandResponse() { Id = record.WarrantyBrand.Id, Name = record.WarrantyBrand.Name },
            Model = record.Model,
            PurchaseDate = record.PurchaseDate,
            Duration = record.Duration,
            EndDate = record.EndDate,
            Status = record.Status,
            Receipt = record.Receipt is not null ?
                new StorageFileResponse()
                {
                    Id = record.Receipt.Id,
                    OriginalFileName = record.Receipt.OriginalFileName,
                    ContentType  = record.Receipt.ContentType,
                    SizeBytes = record.Receipt.SizeBytes,
                } : null,
            FrontImage = record.FrontImage is not null ?
                new StorageFileResponse()
                {
                    Id = record.FrontImage.Id,
                    OriginalFileName = record.FrontImage.OriginalFileName,
                    ContentType = record.FrontImage.ContentType,
                    SizeBytes = record.FrontImage.SizeBytes,
                } : null,
            BackImage = record.BackImage is not null ?
                new StorageFileResponse()
                {
                    Id = record.BackImage.Id,
                    OriginalFileName = record.BackImage.OriginalFileName,
                    ContentType = record.BackImage.ContentType,
                    SizeBytes = record.BackImage.SizeBytes,
                } : null,
        };

    public static WarrantyRecordResponse FillFileUrls(
        this WarrantyRecordResponse response,
        WarrantyRecord record,
        IFileStorage fileStorage)
    {
        SetUrl(response.Receipt, record.Receipt, fileStorage);
        SetUrl(response.FrontImage, record.FrontImage, fileStorage);
        SetUrl(response.BackImage, record.BackImage, fileStorage);
        return response;
    }

    private static void SetUrl(StorageFileResponse? response, StorageFile? file, IFileStorage fileStorage)
    {
        if (response is not null && file is not null)
        {
            response.Url = fileStorage.GetReadUrl(file.BlobName, file.IsPrivate).ToString();
        }
    }
}

public sealed record WarrantyBrandResponse : IMapFrom<WarrantyBrand>
{
    public string Id { get; init; }
    public string Name { get; init; }
}

public sealed record StorageFileResponse : IMapFrom<StorageFile>
{
    public string Id { get; init; }
    public string OriginalFileName { get; init; }
    public string ContentType { get; init; }
    public long SizeBytes { get; init; }
    public string? Url { get; set; }
} 
