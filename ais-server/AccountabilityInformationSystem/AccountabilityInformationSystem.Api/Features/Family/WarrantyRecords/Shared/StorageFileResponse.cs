using AccountabilityInformationSystem.Api.Domain.Entities.Common;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Shared;

public sealed record StorageFileResponse : IMapFrom<StorageFile>
{
    public string Id { get; init; }
    public string OriginalFileName { get; init; }
    public string ContentType { get; init; }
    public long SizeBytes { get; init; }
    public string? Url { get; set; }
}
