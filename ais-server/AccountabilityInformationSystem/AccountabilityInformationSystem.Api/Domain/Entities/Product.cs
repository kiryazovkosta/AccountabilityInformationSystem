using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Excise;

namespace AccountabilityInformationSystem.Api.Domain.Entities;

public class Product : AuditableEntity, IEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public string Code { get; set; }
    public bool IsExcised { get; set; }
    public string ProductTypeId { get; set; }
    public ProductType ProductType { get; set; }
    public string? ApCodeId { get; set; }
    public ApCode? ApCode { get; set; }
    public string? BrandNameId { get; set; }
    public BrandName? BrandName { get; set; }
    public string? CnCodeId { get; set; }
    public CnCode? CnCode { get; set; }
}
