using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Samples.Commands;

namespace LIMS.Application.Samples.Validators;

public sealed class ReceiveSampleCommandValidator
    : AbstractValidator<ReceiveSampleCommand>
{
    public ReceiveSampleCommandValidator()
    {
        RuleFor(x => x.SampleId)
            .NotEmpty()
            .WithMessage("Sample identifier is required.");
    }
}

public sealed class StartSamplePreparationCommandValidator
    : AbstractValidator<StartSamplePreparationCommand>
{
    public StartSamplePreparationCommandValidator()
    {
        RuleFor(x => x.SampleId)
            .NotEmpty()
            .WithMessage("Sample identifier is required.");
    }
}

public sealed class MarkSampleReadyForAnalysisCommandValidator
    : AbstractValidator<MarkSampleReadyForAnalysisCommand>
{
    public MarkSampleReadyForAnalysisCommandValidator()
    {
        RuleFor(x => x.SampleId)
            .NotEmpty()
            .WithMessage("Sample identifier is required.");
    }
}

public sealed class StartSampleAnalysisCommandValidator
    : AbstractValidator<StartSampleAnalysisCommand>
{
    public StartSampleAnalysisCommandValidator()
    {
        RuleFor(x => x.SampleId)
            .NotEmpty()
            .WithMessage("Sample identifier is required.");
    }
}

public sealed class CompleteSampleAnalysisCommandValidator
    : AbstractValidator<CompleteSampleAnalysisCommand>
{
    public CompleteSampleAnalysisCommandValidator()
    {
        RuleFor(x => x.SampleId)
            .NotEmpty()
            .WithMessage("Sample identifier is required.");
    }
}

public sealed class UnassignSampleCommandValidator
    : AbstractValidator<UnassignSampleCommand>
{
    public UnassignSampleCommandValidator()
    {
        RuleFor(x => x.SampleId)
            .NotEmpty()
            .WithMessage("Sample identifier is required.");
    }
}

public sealed class ArchiveSampleCommandValidator
    : AbstractValidator<ArchiveSampleCommand>
{
    public ArchiveSampleCommandValidator()
    {
        RuleFor(x => x.SampleId)
            .NotEmpty()
            .WithMessage("Sample identifier is required.");
    }
}
