using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Experiments.Queries;

namespace LIMS.Application.Experiments.Validators;

public sealed class GetExperimentByIdQueryValidator
    : AbstractValidator<GetExperimentByIdQuery>
{
    public GetExperimentByIdQueryValidator()
    {
        RuleFor(x => x.ExperimentId)
            .Must(x => x != Guid.Empty, "ExperimentId is required.");
    }
}

public sealed class GetExperimentByCodeQueryValidator
    : AbstractValidator<GetExperimentByCodeQuery>
{
    public GetExperimentByCodeQueryValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .Must(x => x != Guid.Empty, "LaboratoryId is required.");

        RuleFor(x => x.ExperimentCode)
            .NotEmpty("ExperimentCode is required.")
            .MaximumLength(50, "ExperimentCode cannot exceed 50 characters.");
    }
}

public sealed class GetExperimentsByLaboratoryIdQueryValidator
    : AbstractValidator<GetExperimentsByLaboratoryIdQuery>
{
    public GetExperimentsByLaboratoryIdQueryValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .Must(x => x != Guid.Empty, "LaboratoryId is required.");
    }
}

public sealed class GetExperimentsBySampleIdQueryValidator
    : AbstractValidator<GetExperimentsBySampleIdQuery>
{
    public GetExperimentsBySampleIdQueryValidator()
    {
        RuleFor(x => x.SampleId)
            .Must(x => x != Guid.Empty, "SampleId is required.");
    }
}

public sealed class GetExperimentsByAssignedUserIdQueryValidator
    : AbstractValidator<GetExperimentsByAssignedUserIdQuery>
{
    public GetExperimentsByAssignedUserIdQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty("UserId is required.")
            .MaximumLength(150, "UserId cannot exceed 150 characters.");
    }
}
