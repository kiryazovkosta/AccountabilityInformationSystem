using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Update;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.ApCodes.Update;

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
