using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetAll;
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

    [Fact]
    public void CreateLinksForMeasuringPoints_ShouldOmitPagingLinks_WhenNoNextOrPreviousPage()
    {
        // Arrange
        GetMeasuringPointsRequest request = new() { Page = 1, PageSize = 10 };

        // Act
        List<LinkResponse> links = _sut.CreateLinksForMeasuringPoints(request, hasNextPage: false, hasPreviousPage: false);

        // Assert
        Assert.Equal(2, links.Count);
        Assert.Contains(links, l => l.Rel == "self");
        Assert.Contains(links, l => l.Rel == "create");
        Assert.DoesNotContain(links, l => l.Rel is "next-page" or "previous-page");
    }

    [Fact]
    public void CreateLinksForMeasuringPoints_ShouldIncludeNextAndPreviousPageLinks_WhenBothExist()
    {
        // Arrange
        GetMeasuringPointsRequest request = new() { Page = 2, PageSize = 10 };

        // Act
        List<LinkResponse> links = _sut.CreateLinksForMeasuringPoints(request, hasNextPage: true, hasPreviousPage: true);

        // Assert
        Assert.Equal(4, links.Count);
        Assert.Contains(links, l => l.Rel == "next-page" && l.Href.Contains("page=3"));
        Assert.Contains(links, l => l.Rel == "previous-page" && l.Href.Contains("page=1"));
    }
}
