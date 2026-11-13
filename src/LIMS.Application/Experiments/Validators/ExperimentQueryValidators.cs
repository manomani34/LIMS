using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Experiments.Queries;

namespace LIMS.Application.Experiments.Validators;

public sealed class GetExperimentByIdQueryValidator
    : AbstractValidator<GetExperimentByIdQuery>
{
    public GetExperimentByIdQueryValidator()
    {
        RuleFor(x => x.ExperimentId)
            .NotEmpty()
            .WithMessage("ExperimentId is required.");
    }
}

public sealed class GetExperimentByCodeQueryValidator
    : AbstractValidator<GetExperimentByCodeQuery>
{
    public GetExperimentByCodeQueryValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .NotEmpty()
            .WithMessage("LaboratoryId is required.");

        RuleFor(x => x.ExperimentCode)
            .NotEmpty()
            .WithMessage("ExperimentCode is required.")
            .MaximumLength(50)
            .WithMessage("ExperimentCode cannot exceed 50 characters.");
    }
}

public sealed class GetExperimentsByLaboratoryIdQueryValidator
    : AbstractValidator<GetExperimentsByLaboratoryIdQuery>
{
    public GetExperimentsByLaboratoryIdQueryValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .NotEmpty()
            .WithMessage("LaboratoryId is required.");
    }
}

public sealed class GetExperimentsBySampleIdQueryValidator
    : AbstractValidator<GetExperimentsBySampleIdQuery>
{
    public GetExperimentsBySampleIdQueryValidator()
    {
        RuleFor(x => x.SampleId)
            .NotEmpty()
            .WithMessage("SampleId is required.");
    }
}

public sealed class GetExperimentsByAssignedUserIdQueryValidator
    : AbstractValidator<GetExperimentsByAssignedUserIdQuery>
{
    public GetExperimentsByAssignedUserIdQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.")
            .MaximumLength(150)
            .WithMessage("UserId cannot exceed 150 characters.");
    }
}
