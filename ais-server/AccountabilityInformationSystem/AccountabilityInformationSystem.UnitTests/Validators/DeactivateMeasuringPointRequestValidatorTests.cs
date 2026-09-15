using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Deactivate;
using FluentValidation.Results;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public sealed class DeactivateMeasuringPointRequestValidatorTests
{
    private readonly DeactivateMeasuringPointRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenActiveToIsToday()
    {
        DeactivateMeasuringPointRequest request = new() { ActiveTo = DateOnly.FromDateTime(DateTime.Today) };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_ShouldSuccess_WhenActiveToIsInTheFuture()
    {
        DeactivateMeasuringPointRequest request = new() { ActiveTo = DateOnly.FromDateTime(DateTime.Today.AddDays(10)) };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenActiveToIsInThePast()
    {
        DeactivateMeasuringPointRequest request = new() { ActiveTo = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)) };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(DeactivateMeasuringPointRequest.ActiveTo), result.Errors.Select(e => e.PropertyName));
    }
}
