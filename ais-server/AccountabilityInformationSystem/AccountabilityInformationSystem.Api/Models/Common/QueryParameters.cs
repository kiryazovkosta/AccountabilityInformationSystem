using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Models.Common;

public record QueryParameters : AcceptHeaderQueryParameter
{
    [FromQuery(Name = "q")]
    public string? Search { get; set; }
    public string? Sort { get; init; }
    public string? Fields { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
