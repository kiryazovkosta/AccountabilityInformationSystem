using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetAll;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetAllV2;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.Linking;
using AccountabilityInformationSystem.UnitTests.Fakes;
using Microsoft.AspNetCore.Http;

namespace AccountabilityInformationSystem.UnitTests.Handlers.Flow.MeasurementPoints;

public sealed class GetMeasuringPointsLinksTests
{
    private readonly LinkService _linkService =
        new(new FakeLinkGenerator(), new HttpContextAccessor { HttpContext = new DefaultHttpContext() });

    [Fact]
    public void CreateLinksForMeasuringPoints_ShouldOmitPagingLinks_WhenNoNextOrPreviousPage()
    {
        // Arrange
        GetMeasuringPointsRequest request = new() { Page = 1, PageSize = 10 };

        // Act
        List<LinkResponse> links = _linkService.CreateLinksForMeasuringPoints(request, hasNextPage: false, hasPreviousPage: false);

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
        List<LinkResponse> links = _linkService.CreateLinksForMeasuringPoints(request, hasNextPage: true, hasPreviousPage: true);

        // Assert
        Assert.Equal(4, links.Count);
        Assert.Contains(links, l => l.Rel == "next-page" && l.Href.Contains("page=3"));
        Assert.Contains(links, l => l.Rel == "previous-page" && l.Href.Contains("page=1"));
    }

    [Fact]
    public void CreateLinksForMeasuringPointsV2_ShouldLinkPagesToTheV2Action()
    {
        // Arrange
        GetMeasuringPointsV2Request request = new() { Page = 2, PageSize = 10 };

        // Act
        List<LinkResponse> links = _linkService.CreateLinksForMeasuringPointsV2(request, hasNextPage: true, hasPreviousPage: true);

        // Assert
        Assert.Equal(4, links.Count);
        Assert.All(
            links.Where(l => l.Rel is "self" or "next-page" or "previous-page"),
            l => Assert.StartsWith("https://fake.test/GetMeasuringPointsV2?", l.Href));
    }
}
