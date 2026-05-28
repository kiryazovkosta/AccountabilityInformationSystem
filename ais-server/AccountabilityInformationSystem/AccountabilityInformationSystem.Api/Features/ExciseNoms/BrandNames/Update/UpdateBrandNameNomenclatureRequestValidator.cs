using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Update;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.BrandNames.Update;

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
