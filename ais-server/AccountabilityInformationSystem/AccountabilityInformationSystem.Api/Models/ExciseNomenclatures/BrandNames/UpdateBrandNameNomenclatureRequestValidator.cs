using AccountabilityInformationSystem.Api.Common.Constants;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Models.ExciseNomenclatures.BrandNames;

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
