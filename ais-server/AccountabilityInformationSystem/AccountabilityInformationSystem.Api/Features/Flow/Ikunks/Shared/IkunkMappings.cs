using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.CreateIkunk;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.UpdateIkunk;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;

internal static class IkunkMappings
{
    public static readonly SortMappingDefinition<IkunkResponse, Ikunk> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(IkunkResponse.Name), nameof(MeasurementPoint.Name)),
            new SortMapping(nameof(IkunkResponse.FullName), nameof(MeasurementPoint.FullName)),
            new SortMapping(nameof(IkunkResponse.Description), nameof(MeasurementPoint.Description)),
            new SortMapping(nameof(IkunkResponse.OrderPosition), nameof(MeasurementPoint.OrderPosition)),
            new SortMapping(nameof(IkunkResponse.ActiveFrom), nameof(MeasurementPoint.ActiveFrom)),
            new SortMapping(nameof(IkunkResponse.ActiveTo), nameof(MeasurementPoint.ActiveTo)),
            new SortMapping(
                $"{nameof(IkunkResponse.Warehouse)}.{nameof(IkunkResponse.Warehouse.Id)}",
                $"{nameof(Ikunk.Warehouse)}.{nameof(Ikunk.Warehouse.Id)}")
        ]
    };

    public static Ikunk ToEntity(this CreateIkunkRequest request, string userName)
        => new()
    {
        Id = $"ik_{Guid.CreateVersion7()}",
        Name = request.Name,
        FullName = request.FullName,
        Description = request.Description,
        OrderPosition = request.OrderPosition,
        ActiveFrom = request.ActiveFrom,
        ActiveTo = request.ActiveTo,
        WarehouseId = request.WarehouseId,
        CreatedBy = userName,
        CreatedAt = DateTime.UtcNow
    };

    public static IkunkResponse ToResponse(this Ikunk ikunk)
        => new()
        {
            Id = ikunk.Id,
            Name = ikunk.Name,
            FullName = ikunk.FullName,
            Description = ikunk.Description,
            OrderPosition = ikunk.OrderPosition,
            ActiveFrom = ikunk.ActiveFrom,
            ActiveTo = ikunk.ActiveTo,
            Warehouse = ikunk.Warehouse is not null ?
                new IkunkWarehouseResponse()
                {
                    Id = ikunk.Warehouse.Id,
                    FullName = ikunk.Warehouse.FullName
                } : null ,
            MeasurementPoints = [.. ikunk.MeasurementPoints
                .OrderBy(mp => mp.OrderPosition)
                .Select(mp => new IkunkMeasurementPointResponse()
                {
                    Id = mp.Id,
                    FullName = mp.FullName
                })]
        };

    public static void UpdateFromRequest(this Ikunk ikunk, UpdateIkunkRequest request, string userName)
    {
        ikunk.Name = request.Name ?? ikunk.Name;
        ikunk.FullName = request.FullName ?? ikunk.FullName;
        ikunk.Description = request.Description ?? ikunk.Description;
        ikunk.OrderPosition = request.OrderPosition ?? ikunk.OrderPosition;
        ikunk.ActiveFrom = request.ActiveFrom ?? ikunk.ActiveFrom;
        ikunk.ActiveTo = request.ActiveTo ?? ikunk.ActiveTo;
        ikunk.WarehouseId = request.WarehouseId ?? ikunk.WarehouseId;
        ikunk.ModifiedBy = userName;
        ikunk.ModifiedAt = DateTime.UtcNow;
    }
}
