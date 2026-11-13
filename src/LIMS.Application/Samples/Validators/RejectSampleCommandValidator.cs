using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Samples.Commands;

namespace LIMS.Application.Samples.Validators;

public sealed class RejectSampleCommandValidator
    : AbstractValidator<RejectSampleCommand>
{
    public RejectSampleCommandValidator()
    {
        RuleFor(x => x.SampleId)
            .NotEmpty()
            .WithMessage("Sample identifier is required.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Rejection reason is required.")
            .MaximumLength(1000)
            .WithMessage("Rejection reason cannot exceed 1000 characters.");
    }
}
