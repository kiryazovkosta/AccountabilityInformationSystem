using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Update;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.ApCodes.Update;

public sealed class UpdateApCodeNomenclatureRequestValidator : AbstractValidator<UpdateApCodeNomenclatureRequest>
{
    public UpdateApCodeNomenclatureRequestValidator()
    {
        Include(new UpdateExciseNomenclatureValidator<UpdateApCodeNomenclatureRequest>(
            EntitiesConstants.ApCodeConstants.CodeLength,
            EntitiesConstants.ApCodeConstants.CodePattern,
            EntitiesConstants.ApCodeConstants.DescriptionMaxlength));
    }
}
