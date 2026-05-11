using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Deactivate;
using FluentValidation.Results;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public sealed class DeactivateMeasuringPointBodyValidatorTests
{
    private readonly DeactivateMeasuringPointBodyValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenActiveToIsToday()
    {
        DeactivateMeasuringPointBody body = new() { ActiveTo = DateOnly.FromDateTime(DateTime.Today) };

        ValidationResult result = await _validator.ValidateAsync(body, CancellationToken.None);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_ShouldSuccess_WhenActiveToIsInTheFuture()
    {
        DeactivateMeasuringPointBody body = new() { ActiveTo = DateOnly.FromDateTime(DateTime.Today.AddDays(10)) };

        ValidationResult result = await _validator.ValidateAsync(body, CancellationToken.None);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenActiveToIsInThePast()
    {
        DeactivateMeasuringPointBody body = new() { ActiveTo = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)) };

        ValidationResult result = await _validator.ValidateAsync(body, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(DeactivateMeasuringPointBody.ActiveTo), result.Errors.Select(e => e.PropertyName));
    }
}
