using AccountabilityInformationSystem.Api.Models.Common;

namespace AccountabilityInformationSystem.Api.Services.Linking;

public sealed class LinkService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
{
    public LinkResponse Create(
        string endpointName,
        string rel,
        string method,
        object? values = null,
        string? controller = null)
    {
        string? href = linkGenerator.GetUriByAction(
            httpContextAccessor.HttpContext!,
            endpointName,
            controller,
            values);

        return new LinkResponse
        {
            Href = href ?? throw new ArgumentException("Invalid endpoint name provided"),
            Rel = rel,
            Method = method
        };
    }
}
