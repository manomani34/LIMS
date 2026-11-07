using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Protocols.Commands;

namespace LIMS.Application.Protocols.Validators;

public sealed class CreateProtocolCommandValidator
    : AbstractValidator<CreateProtocolCommand>
{
    public CreateProtocolCommandValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .Must(x => x != Guid.Empty, "LaboratoryId is required.");

        RuleFor(x => x.ProtocolCode)
            .NotEmpty("ProtocolCode is required.")
            .MaximumLength(50, "ProtocolCode cannot exceed 50 characters.");

        RuleFor(x => x.Name)
            .NotEmpty("Name is required.")
            .MaximumLength(200, "Name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(4000, "Description cannot exceed 4000 characters.");

        RuleFor(x => x.Version)
            .NotEmpty("Version is required.")
            .MaximumLength(50, "Version cannot exceed 50 characters.");

        RuleFor(x => x.CreatedByUserId)
            .NotEmpty("CreatedByUserId is required.")
            .MaximumLength(150, "CreatedByUserId cannot exceed 150 characters.");
    }
}
