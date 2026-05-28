using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.ApCodes.Create;

public sealed class CreateApCodeNomenclatureRequestValidator : AbstractValidator<CreateApCodeNomenclatureRequest>
{
    public CreateApCodeNomenclatureRequestValidator()
    {
        Include(new CreateExciseNomenclatureValidator<CreateApCodeNomenclatureRequest>(
            EntitiesConstants.ApCodeConstants.CodePattern,
            EntitiesConstants.ApCodeConstants.DescriptionMaxlength));
    }
}
