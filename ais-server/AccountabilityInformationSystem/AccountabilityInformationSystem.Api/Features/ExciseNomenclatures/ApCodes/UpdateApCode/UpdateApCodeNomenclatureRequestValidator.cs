using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.UpdateExciseNomenclature;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.ApCodes.UpdateApCode;

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
