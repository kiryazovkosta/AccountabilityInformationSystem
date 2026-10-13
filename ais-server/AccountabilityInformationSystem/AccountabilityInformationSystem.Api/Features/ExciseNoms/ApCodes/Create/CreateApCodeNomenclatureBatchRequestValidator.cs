using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.ApCodes.Create;

public sealed class CreateApCodeNomenclatureBatchRequestValidator
    : AbstractValidator<CreateExciseNomenclatureBatchRequest<CreateApCodeNomenclatureRequest>>
{
    public CreateApCodeNomenclatureBatchRequestValidator(IValidator<CreateApCodeNomenclatureRequest> entryValidator)
    {
        Include(new CreateExciseNomenclatureBatchRequestValidator<CreateApCodeNomenclatureRequest>(entryValidator));
    }
}
