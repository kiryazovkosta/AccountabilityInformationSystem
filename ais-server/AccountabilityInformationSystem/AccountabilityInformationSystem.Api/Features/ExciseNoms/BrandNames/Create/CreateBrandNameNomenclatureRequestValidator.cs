using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.BrandNames.Create;

public sealed class CreateBrandNameNomenclatureRequestValidator : AbstractValidator<CreateBrandNameNomenclatureRequest>
{
    public CreateBrandNameNomenclatureRequestValidator()
    {
        Include(new CreateExciseNomenclatureValidator<CreateBrandNameNomenclatureRequest>(
            EntitiesConstants.BrandNameConstants.CodePattern,
            EntitiesConstants.BrandNameConstants.DescriptionMaxlength));
    }
}
