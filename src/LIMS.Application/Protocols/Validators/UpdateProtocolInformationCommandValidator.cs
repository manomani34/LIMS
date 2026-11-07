using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Protocols.Commands;

namespace LIMS.Application.Protocols.Validators;

public sealed class UpdateProtocolInformationCommandValidator
    : AbstractValidator<UpdateProtocolInformationCommand>
{
    public UpdateProtocolInformationCommandValidator()
    {
        RuleFor(x => x.ProtocolId)
            .Must(x => x != Guid.Empty, "ProtocolId is required.");

        RuleFor(x => x.Name)
            .NotEmpty("Name is required.")
            .MaximumLength(200, "Name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(4000, "Description cannot exceed 4000 characters.");

        RuleFor(x => x.Version)
            .NotEmpty("Version is required.")
            .MaximumLength(50, "Version cannot exceed 50 characters.");
    }
}
