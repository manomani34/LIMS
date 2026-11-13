using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Protocols.Queries;

namespace LIMS.Application.Protocols.Validators;

public sealed class GetProtocolByIdQueryValidator
    : AbstractValidator<GetProtocolByIdQuery>
{
    public GetProtocolByIdQueryValidator()
    {
        RuleFor(x => x.ProtocolId)
            .NotEmpty()
            .WithMessage("ProtocolId is required.");
    }
}

public sealed class GetProtocolByCodeQueryValidator
    : AbstractValidator<GetProtocolByCodeQuery>
{
    public GetProtocolByCodeQueryValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .NotEmpty()
            .WithMessage("LaboratoryId is required.");

        RuleFor(x => x.ProtocolCode)
            .NotEmpty()
            .WithMessage("ProtocolCode is required.")
            .MaximumLength(50)
            .WithMessage("ProtocolCode cannot exceed 50 characters.");
    }
}

public sealed class GetProtocolsByLaboratoryIdQueryValidator
    : AbstractValidator<GetProtocolsByLaboratoryIdQuery>
{
    public GetProtocolsByLaboratoryIdQueryValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .NotEmpty()
            .WithMessage("LaboratoryId is required.");
    }
}
