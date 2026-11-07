using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Samples.Commands;

namespace LIMS.Application.Samples.Validators;

public sealed class ReceiveSampleCommandValidator
    : AbstractValidator<ReceiveSampleCommand>
{
    public ReceiveSampleCommandValidator()
    {
        RuleFor(command => command.SampleId, (command, failures) =>
        {
            if (command.SampleId == Guid.Empty)
                failures.Add(new ValidationFailure(
                    nameof(command.SampleId),
                    "Sample identifier is required."));
        });
    }
}

public sealed class StartSamplePreparationCommandValidator
    : AbstractValidator<StartSamplePreparationCommand>
{
    public StartSamplePreparationCommandValidator()
    {
        RuleFor(command => command.SampleId, (command, failures) =>
        {
            if (command.SampleId == Guid.Empty)
                failures.Add(new ValidationFailure(
                    nameof(command.SampleId),
                    "Sample identifier is required."));
        });
    }
}

public sealed class MarkSampleReadyForAnalysisCommandValidator
    : AbstractValidator<MarkSampleReadyForAnalysisCommand>
{
    public MarkSampleReadyForAnalysisCommandValidator()
    {
        RuleFor(command => command.SampleId, (command, failures) =>
        {
            if (command.SampleId == Guid.Empty)
                failures.Add(new ValidationFailure(
                    nameof(command.SampleId),
                    "Sample identifier is required."));
        });
    }
}

public sealed class StartSampleAnalysisCommandValidator
    : AbstractValidator<StartSampleAnalysisCommand>
{
    public StartSampleAnalysisCommandValidator()
    {
        RuleFor(command => command.SampleId, (command, failures) =>
        {
            if (command.SampleId == Guid.Empty)
                failures.Add(new ValidationFailure(
                    nameof(command.SampleId),
                    "Sample identifier is required."));
        });
    }
}

public sealed class CompleteSampleAnalysisCommandValidator
    : AbstractValidator<CompleteSampleAnalysisCommand>
{
    public CompleteSampleAnalysisCommandValidator()
    {
        RuleFor(command => command.SampleId, (command, failures) =>
        {
            if (command.SampleId == Guid.Empty)
                failures.Add(new ValidationFailure(
                    nameof(command.SampleId),
                    "Sample identifier is required."));
        });
    }
}

public sealed class UnassignSampleCommandValidator
    : AbstractValidator<UnassignSampleCommand>
{
    public UnassignSampleCommandValidator()
    {
        RuleFor(command => command.SampleId, (command, failures) =>
        {
            if (command.SampleId == Guid.Empty)
                failures.Add(new ValidationFailure(
                    nameof(command.SampleId),
                    "Sample identifier is required."));
        });
    }
}

public sealed class ArchiveSampleCommandValidator
    : AbstractValidator<ArchiveSampleCommand>
{
    public ArchiveSampleCommandValidator()
    {
        RuleFor(command => command.SampleId, (command, failures) =>
        {
            if (command.SampleId == Guid.Empty)
                failures.Add(new ValidationFailure(
                    nameof(command.SampleId),
                    "Sample identifier is required."));
        });
    }
}
