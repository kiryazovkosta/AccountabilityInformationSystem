using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

namespace AccountabilityInformationSystem.Api.Domain.Entities.Excise;

public class CnCode : AuditableEntity, IEntity, IExciseEntity
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string BgDescription { get; set; }
    public string? DescriptionEn { get; set; }
    public bool IsUsed { get; set; }
    public ICollection<Product> Products { get; set; } = [];
}
