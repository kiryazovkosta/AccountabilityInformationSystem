using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Shared;

public sealed record WarrantyRecordResponse : IMapFrom<WarrantyRecord>
{
    public required string Id { get; init; }
    public required WarrantyBrandResponse WarrantyBrand { get; init; }
    public required string Model { get; init; }
    public required DateOnly PurchaseDate { get; init; }
    public StorageFileResponse? Receipt { get; init; }
    public StorageFileResponse? FrontImage { get; init; }
    public StorageFileResponse? BackImage { get; init; }
}

public sealed record WarrantyRecordListResponse : IMapFrom<WarrantyRecord>, IMapCustom
{
    public required string Id { get; init; }
    public required string WarrantyBrandName { get; init; }
    public required string Model { get; init; }
    public required DateOnly PurchaseDate { get; init; }
    public bool ReceiptExists { get; init; }
    public bool FrontImageExists { get; init; }
    public bool BackImageExists { get; init; }

    public void CreateMappings(Mapster.TypeAdapterConfig config)
    {
        config.NewConfig<WarrantyRecord, WarrantyRecordListResponse>()
            .Map(dest => dest.ReceiptExists, src => src.Receipt != null)
            .Map(dest => dest.FrontImageExists, src => src.FrontImage != null)
            .Map(dest => dest.BackImageExists, src => src.BackImage != null);
    }
}
