using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.UpdateExciseNomenclature;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.BrandNames.UpdateBrandName;

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
