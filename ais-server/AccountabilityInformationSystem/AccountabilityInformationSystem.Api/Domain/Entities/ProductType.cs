using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

namespace AccountabilityInformationSystem.Api.Domain.Entities;

public class ProductType : AuditableEntity, IEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public ICollection<Product> Products { get; set; } = [];
}
