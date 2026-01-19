using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.CreateExciseNomenclature;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.BrandNames.CreateBrandName;

public sealed class CreateBrandNameNomenclatureRequestValidator : AbstractValidator<CreateBrandNameNomenclatureRequest>
{
    public CreateBrandNameNomenclatureRequestValidator()
    {
        Include(new CreateExciseNomenclatureValidator<CreateBrandNameNomenclatureRequest>(
            EntitiesConstants.BrandNameConstants.CodePattern,
            EntitiesConstants.BrandNameConstants.DescriptionMaxlength));
    }
}
