using AccountabilityInformationSystem.Api.Common.Constants;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Models.ExciseNomenclatures.ApCodes;

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
