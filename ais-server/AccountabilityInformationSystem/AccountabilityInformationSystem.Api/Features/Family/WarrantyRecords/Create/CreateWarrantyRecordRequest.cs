using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Create;

public sealed record CreateWarrantyRecordRequest : IMapTo<WarrantyRecord>, IMapCustom
{
    public required string WarrantyBrandId { get; init; }
    public required string Model { get; init; }
    public required DateOnly PurchaseDate { get; init; }
    public IFormFile? Receipt { get; init; }
    public IFormFile? FrontImage { get; init; }
    public IFormFile? BackImage { get; init; }
    public required int Duration { get; init; }

    public void CreateMappings(TypeAdapterConfig config) =>
        config.NewConfig<CreateWarrantyRecordRequest, WarrantyRecord>()
            .Map(dest => dest.Id, _ => $"wr_{Guid.CreateVersion7()}")
            .Ignore(dest => dest.Receipt)
            .Ignore(dest => dest.FrontImage)
            .Ignore(dest => dest.BackImage);
}
