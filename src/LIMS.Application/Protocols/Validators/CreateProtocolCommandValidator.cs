using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Protocols.Commands;

namespace LIMS.Application.Protocols.Validators;

public sealed class CreateProtocolCommandValidator
    : AbstractValidator<CreateProtocolCommand>
{
    public CreateProtocolCommandValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .NotEmpty()
            .WithMessage("LaboratoryId is required.");

        RuleFor(x => x.ProtocolCode)
            .NotEmpty()
            .WithMessage("ProtocolCode is required.")
            .MaximumLength(50)
            .WithMessage("ProtocolCode cannot exceed 50 characters.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(200)
            .WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(4000)
            .WithMessage("Description cannot exceed 4000 characters.");

        RuleFor(x => x.Version)
            .NotEmpty()
            .WithMessage("Version is required.")
            .MaximumLength(50)
            .WithMessage("Version cannot exceed 50 characters.");

        RuleFor(x => x.CreatedByUserId)
            .NotEmpty()
            .WithMessage("CreatedByUserId is required.")
            .MaximumLength(150)
            .WithMessage("CreatedByUserId cannot exceed 150 characters.");
    }
}
