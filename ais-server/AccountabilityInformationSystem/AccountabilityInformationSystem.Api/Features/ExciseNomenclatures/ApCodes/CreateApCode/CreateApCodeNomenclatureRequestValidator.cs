using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.CreateExciseNomenclature;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.ApCodes.CreateApCode;

public sealed class CreateApCodeNomenclatureRequestValidator : AbstractValidator<CreateApCodeNomenclatureRequest>
{
    public CreateApCodeNomenclatureRequestValidator()
    {
        Include(new CreateExciseNomenclatureValidator<CreateApCodeNomenclatureRequest>(
            EntitiesConstants.ApCodeConstants.CodePattern,
            EntitiesConstants.ApCodeConstants.DescriptionMaxlength));
    }
}
