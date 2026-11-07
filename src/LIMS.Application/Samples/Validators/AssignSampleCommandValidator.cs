using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Samples.Commands;

namespace LIMS.Application.Samples.Validators;

public sealed class AssignSampleCommandValidator
    : AbstractValidator<AssignSampleCommand>
{
    public AssignSampleCommandValidator()
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
            command => command.UserId,
            (command, failures) =>
            {
                if (string.IsNullOrWhiteSpace(command.UserId))
                    failures.Add(new ValidationFailure(
                        nameof(command.UserId),
                        "User identifier is required."));
            });
    }
}
