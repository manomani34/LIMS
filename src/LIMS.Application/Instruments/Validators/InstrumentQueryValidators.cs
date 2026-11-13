using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Instruments.Queries;

namespace LIMS.Application.Instruments.Validators;

public sealed class GetInstrumentByIdQueryValidator
    : AbstractValidator<GetInstrumentByIdQuery>
{
    public GetInstrumentByIdQueryValidator()
    {
        RuleFor(x => x.InstrumentId).NotEmpty();
    }
}

public sealed class GetInstrumentByCodeQueryValidator
    : AbstractValidator<GetInstrumentByCodeQuery>
{
    public GetInstrumentByCodeQueryValidator()
    {
        RuleFor(x => x.LaboratoryId).NotEmpty();

        RuleFor(x => x.InstrumentCode)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public sealed class GetInstrumentsByLaboratoryIdQueryValidator
    : AbstractValidator<GetInstrumentsByLaboratoryIdQuery>
{
    public GetInstrumentsByLaboratoryIdQueryValidator()
    {
        RuleFor(x => x.LaboratoryId).NotEmpty();
    }
}

public sealed class GetInstrumentsByStatusQueryValidator
    : AbstractValidator<GetInstrumentsByStatusQuery>
{
    public GetInstrumentsByStatusQueryValidator()
    {
        RuleFor(x => x.LaboratoryId).NotEmpty();

        RuleFor(x => x.Status)
            .Must(Enum.IsDefined);
    }
}

