using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.Linking;
using AccountabilityInformationSystem.UnitTests.Fakes;
using Microsoft.AspNetCore.Http;

namespace AccountabilityInformationSystem.UnitTests.Handlers.Flow.Shared;

public sealed class MeasuringPointLinkServiceTests
{
    private readonly MeasuringPointLinkService _sut = new(
        new LinkService(new FakeLinkGenerator(), new HttpContextAccessor { HttpContext = new DefaultHttpContext() }));

    [Fact]
    public void CreateLinksForMeasuringPoint_ShouldReturnSelfUpdateAndDeactivateLinks()
    {
        // Act
        List<LinkResponse> links = _sut.CreateLinksForMeasuringPoint("mp_1", "name,fullName");

        // Assert
        Assert.Equal(3, links.Count);
        Assert.Contains(links, l => l.Rel == "self" && l.Method == HttpMethods.Get && l.Href.Contains("id=mp_1"));
        Assert.Contains(links, l => l.Rel == "update" && l.Method == HttpMethods.Put);
        Assert.Contains(links, l => l.Rel == "deactivate" && l.Method == HttpMethods.Put);
    }
}
