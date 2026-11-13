
using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Experiments.Commands;

namespace LIMS.Application.Experiments.Validators;

public sealed class UnassignExperimentCommandValidator
    : AbstractValidator<UnassignExperimentCommand>
{
    public UnassignExperimentCommandValidator()
    {
        RuleFor(x => x.ExperimentId)
            .NotEmpty()
            .WithMessage("ExperimentId is required.");
    }
}

public sealed class StartExperimentCommandValidator
    : AbstractValidator<StartExperimentCommand>
{
    public StartExperimentCommandValidator()
    {
        RuleFor(x => x.ExperimentId)
            .NotEmpty()
            .WithMessage("ExperimentId is required.");
    }
}

public sealed class CompleteExperimentCommandValidator
    : AbstractValidator<CompleteExperimentCommand>
{
    public CompleteExperimentCommandValidator()
    {
        RuleFor(x => x.ExperimentId)
            .NotEmpty()
            .WithMessage("ExperimentId is required.");
    }
}

public sealed class ArchiveExperimentCommandValidator
    : AbstractValidator<ArchiveExperimentCommand>
{
    public ArchiveExperimentCommandValidator()
    {
        RuleFor(x => x.ExperimentId)
            .NotEmpty()
            .WithMessage("ExperimentId is required.");
    }
}
