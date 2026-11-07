using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Protocols.Queries;

namespace LIMS.Application.Protocols.Validators;

public sealed class GetProtocolByIdQueryValidator
    : AbstractValidator<GetProtocolByIdQuery>
{
    public GetProtocolByIdQueryValidator()
    {
        RuleFor(x => x.ProtocolId)
            .Must(x => x != Guid.Empty, "ProtocolId is required.");
    }
}

public sealed class GetProtocolByCodeQueryValidator
    : AbstractValidator<GetProtocolByCodeQuery>
{
    public GetProtocolByCodeQueryValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .Must(x => x != Guid.Empty, "LaboratoryId is required.");

        RuleFor(x => x.ProtocolCode)
            .NotEmpty("ProtocolCode is required.")
            .MaximumLength(50, "ProtocolCode cannot exceed 50 characters.");
    }
}

public sealed class GetProtocolsByLaboratoryIdQueryValidator
    : AbstractValidator<GetProtocolsByLaboratoryIdQuery>
{
    public GetProtocolsByLaboratoryIdQueryValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .Must(x => x != Guid.Empty, "LaboratoryId is required.");
    }
}
