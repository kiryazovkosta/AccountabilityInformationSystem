using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.GetAll;
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
    }

    [Fact]
    public void CreateLinksForMeasuringPointsData_ShouldOmitPagingLinks_WhenNoNextOrPreviousPage()
    {
        // Arrange
        GetMeasurementPointsDataRequest request = new() { Page = 1, PageSize = 10 };

        // Act
        List<LinkResponse> links = _sut.CreateLinksForMeasuringPointsData(request, hasNextPage: false, hasPreviousPage: false);

        // Assert
        Assert.Equal(2, links.Count);
        Assert.Contains(links, l => l.Rel == "self");
        Assert.Contains(links, l => l.Rel == "create");
        Assert.DoesNotContain(links, l => l.Rel is "next-page" or "previous-page");
    }

    [Fact]
    public void CreateLinksForMeasuringPointsData_ShouldIncludeNextAndPreviousPageLinks_WhenBothExist()
    {
        // Arrange
        GetMeasurementPointsDataRequest request = new() { Page = 2, PageSize = 10 };

        // Act
        List<LinkResponse> links = _sut.CreateLinksForMeasuringPointsData(request, hasNextPage: true, hasPreviousPage: true);

        // Assert
        Assert.Equal(4, links.Count);
        Assert.Contains(links, l => l.Rel == "next-page" && l.Href.Contains("page=3"));
        Assert.Contains(links, l => l.Rel == "previous-page" && l.Href.Contains("page=1"));
    }
}
