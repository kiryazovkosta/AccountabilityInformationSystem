using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

namespace AccountabilityInformationSystem.Api.Domain.Entities.Common;

public sealed class StorageFile : AuditableEntity, IEntity
{
    public string Id { get; set; }
    public string BlobName { get; set; } = null!;
    public bool IsPrivate { get; set; }
    public string OriginalFileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long SizeBytes { get; set; }
}
