using AccountabilityInformationSystem.Api.Models.Common;

namespace AccountabilityInformationSystem.Api.Models.Warehouses;

public sealed record WarehousesCollectionResponse : ICollectionResponse<WarehouseResponse>
{
    public List<WarehouseResponse> Items { get; init; }
}
