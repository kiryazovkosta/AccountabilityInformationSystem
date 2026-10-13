using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.Linking;
using AccountabilityInformationSystem.UnitTests.Fakes;
using Microsoft.AspNetCore.Http;

namespace AccountabilityInformationSystem.UnitTests.Handlers.Flow.Shared;

public sealed class MeasuringPointDataLinkServiceTests
{
    private readonly MeasuringPointDataLinkService _sut = new(
        new LinkService(new FakeLinkGenerator(), new HttpContextAccessor { HttpContext = new DefaultHttpContext() }));

    [Fact]
    public void CreateLinksForMeasuringPointData_ShouldReturnSingleSelfLink()
    {
        // Act
        List<LinkResponse> links = _sut.CreateLinksForMeasuringPointData("md_1", "number,beginTime");

        // Assert
        LinkResponse link = Assert.Single(links);
        Assert.Equal("self", link.Rel);
        Assert.Equal(HttpMethods.Get, link.Method);
        Assert.Contains("id=md_1", link.Href);
        Assert.StartsWith("https://fake.test/GetMeasuringPointData?", link.Href);
    }
}
