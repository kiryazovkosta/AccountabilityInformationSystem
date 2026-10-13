using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.CnCodes.Create;

public sealed class CreateCnCodeNomenclatureBatchRequestValidator
    : AbstractValidator<CreateExciseNomenclatureBatchRequest<CreateCnCodeNomenclatureRequest>>
{
    public CreateCnCodeNomenclatureBatchRequestValidator(IValidator<CreateCnCodeNomenclatureRequest> entryValidator)
    {
        Include(new CreateExciseNomenclatureBatchRequestValidator<CreateCnCodeNomenclatureRequest>(entryValidator));
    }
}
