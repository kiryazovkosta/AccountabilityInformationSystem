using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Models;
using Mapster;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetTransports;

public sealed class GetTransportsRequestHandler
{
    public Result<List<EnumTypeResponse>> Handle(GetTransportsRequest _)
    {
        List<EnumTypeResponse> transports = [.. Enum
            .GetValues<TransportType>()
            .Select(t => t.Adapt<EnumTypeResponse>())];
        return Result<List<EnumTypeResponse>>.Success(transports);
    }
}
