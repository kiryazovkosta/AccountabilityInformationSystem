using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Update;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.BrandNames.Update;

public sealed class UpdateBrandNameNomenclatureRequestValidator : AbstractValidator<UpdateBrandNameNomenclatureRequest>
{
    public UpdateBrandNameNomenclatureRequestValidator()
    {
        Include(new UpdateExciseNomenclatureValidator<UpdateBrandNameNomenclatureRequest>(
            EntitiesConstants.BrandNameConstants.CodeLength,
            EntitiesConstants.BrandNameConstants.CodePattern,
            EntitiesConstants.BrandNameConstants.DescriptionMaxlength));
    }
}
