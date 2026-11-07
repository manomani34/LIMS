using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Samples.Commands;

namespace LIMS.Application.Samples.Validators;

public sealed class RegisterSampleCommandValidator
    : AbstractValidator<RegisterSampleCommand>
{
    public RegisterSampleCommandValidator()
    {
        RuleFor(
            command => command.LaboratoryId,
            (command, failures) =>
            {
                if (command.LaboratoryId == Guid.Empty)
                    failures.Add(new ValidationFailure(
                        nameof(command.LaboratoryId),
                        "Laboratory identifier is required."));
            });

        RuleFor(
            command => command.SampleCode,
            (command, failures) =>
            {
                if (string.IsNullOrWhiteSpace(command.SampleCode))
                    failures.Add(new ValidationFailure(
                        nameof(command.SampleCode),
                        "Sample code is required."));
                else if (command.SampleCode.Length > 50)
                    failures.Add(new ValidationFailure(
                        nameof(command.SampleCode),
                        "Sample code cannot exceed 50 characters."));
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

        RuleFor(
            command => command.RegisteredByUserId,
            (command, failures) =>
            {
                if (string.IsNullOrWhiteSpace(command.RegisteredByUserId))
                    failures.Add(new ValidationFailure(
                        nameof(command.RegisteredByUserId),
                        "Registering user identifier is required."));
            });

        RuleFor(
            command => command.Type,
            (command, failures) =>
            {
                if (!Enum.IsDefined(command.Type))
                    failures.Add(new ValidationFailure(
                        nameof(command.Type),
                        "Sample type is invalid."));
            });

        RuleFor(
            command => command.Priority,
            (command, failures) =>
            {
                if (!Enum.IsDefined(command.Priority))
                    failures.Add(new ValidationFailure(
                        nameof(command.Priority),
                        "Sample priority is invalid."));
            });
    }
}
