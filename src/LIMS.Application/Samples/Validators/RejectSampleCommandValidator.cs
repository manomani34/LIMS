using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Samples.Commands;

namespace LIMS.Application.Samples.Validators;

public sealed class RejectSampleCommandValidator
    : AbstractValidator<RejectSampleCommand>
{
    public RejectSampleCommandValidator()
    {
        RuleFor(
            command => command.SampleId,
            (command, failures) =>
            {
                if (command.SampleId == Guid.Empty)
                    failures.Add(new ValidationFailure(
                        nameof(command.SampleId),
                        "Sample identifier is required."));
            });

        RuleFor(
            command => command.Reason,
            (command, failures) =>
            {
                if (string.IsNullOrWhiteSpace(command.Reason))
                    failures.Add(new ValidationFailure(
                        nameof(command.Reason),
                        "Rejection reason is required."));
                else if (command.Reason.Length > 1000)
                    failures.Add(new ValidationFailure(
                        nameof(command.Reason),
                        "Rejection reason cannot exceed 1000 characters."));
            });
    }
}
