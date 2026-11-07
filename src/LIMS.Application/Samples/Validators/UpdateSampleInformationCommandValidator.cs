using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Samples.Commands;

namespace LIMS.Application.Samples.Validators;

public sealed class UpdateSampleInformationCommandValidator
    : AbstractValidator<UpdateSampleInformationCommand>
{
    public UpdateSampleInformationCommandValidator()
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
            command => command.Name,
            (command, failures) =>
            {
                if (string.IsNullOrWhiteSpace(command.Name))
                    failures.Add(new ValidationFailure(
                        nameof(command.Name),
                        "Sample name is required."));
                else if (command.Name.Length > 200)
                    failures.Add(new ValidationFailure(
                        nameof(command.Name),
                        "Sample name cannot exceed 200 characters."));
            });

        RuleFor(
            command => command.Description,
            (command, failures) =>
            {
                if (command.Description?.Length > 2000)
                    failures.Add(new ValidationFailure(
                        nameof(command.Description),
                        "Description cannot exceed 2000 characters."));
            });

        RuleFor(
            command => command.Source,
            (command, failures) =>
            {
                if (command.Source?.Length > 250)
                    failures.Add(new ValidationFailure(
                        nameof(command.Source),
                        "Source cannot exceed 250 characters."));
            });
    }
}
