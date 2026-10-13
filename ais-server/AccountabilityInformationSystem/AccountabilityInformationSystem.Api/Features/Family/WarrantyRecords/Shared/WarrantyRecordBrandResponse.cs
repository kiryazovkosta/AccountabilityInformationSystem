using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Shared;

public sealed record WarrantyRecordBrandResponse : IMapFrom<WarrantyBrand>
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Logo { get; init; }
}
