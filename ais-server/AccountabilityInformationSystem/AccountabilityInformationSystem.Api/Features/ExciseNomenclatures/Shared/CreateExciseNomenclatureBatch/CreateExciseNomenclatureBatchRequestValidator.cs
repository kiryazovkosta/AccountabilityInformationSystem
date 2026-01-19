using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.CreateExciseNomenclature;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.CreateExciseNomenclatureBatch;

public sealed class CreateExciseNomenclatureBatchRequestValidator<TCreateRequest>
    : AbstractValidator<CreateExciseNomenclatureBatchRequest<TCreateRequest>>
    where TCreateRequest : CreateExciseNomenclatureRequest
{
    public CreateExciseNomenclatureBatchRequestValidator(IValidator<TCreateRequest> entryValidator)
    {
        RuleFor(x => x.Entries)
            .NotNull()
            .WithMessage("Entries list cannot be null.");

        RuleFor(x => x.Entries)
            .NotEmpty()
            .WithMessage("Entries list cannot be empty.");

        RuleForEach(x => x.Entries)
            .SetValidator(entryValidator);
    }
}
