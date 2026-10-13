using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Models;
using Mapster;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetFlowDirections;

public sealed class GetFlowDirectionsRequestHandler
{
    public Result<List<EnumTypeResponse>> Handle(GetFlowDirectionsRequest _)
    {
        List<EnumTypeResponse> flowDirections = [.. Enum
            .GetValues<FlowDirectionType>()
            .Select(t => t.Adapt<EnumTypeResponse>())];
        return Result<List<EnumTypeResponse>>.Success(flowDirections);
    }
}
