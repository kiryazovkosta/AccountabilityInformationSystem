using AccountabilityInformationSystem.Api.Domain.Entities;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.ProductTypes.Shared;

public sealed record ProductTypeProductResponse : IMapFrom<Product>
{
    public string Id { get; init; }
    public string Code { get; init; }
    public string FullName { get; init; }
}
