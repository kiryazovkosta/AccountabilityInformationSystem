using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Update;
using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation.Results;

namespace AccountabilityInformationSystem.UnitTests.Validators;

public sealed class UpdateIkunkRequestValidatorTests
{
    private readonly UpdateIkunkRequestValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSuccess_WhenFullNameIsLongerThanANameButWithinFullNameLimit()
    {
        UpdateIkunkRequest request = new() { FullName = new string('a', EntitiesConstants.NameMaxLength + 1) };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenFullNameExceedsFullNameLimit()
    {
        UpdateIkunkRequest request = new() { FullName = new string('a', EntitiesConstants.FullNameMaxLength + 1) };

        ValidationResult result = await _validator.ValidateAsync(request, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(nameof(UpdateIkunkRequest.FullName), result.Errors.Select(e => e.PropertyName));
    }
}
