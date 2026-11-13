using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Samples.Queries;

namespace LIMS.Application.Samples.Validators;

public sealed class GetSampleByIdQueryValidator
    : AbstractValidator<GetSampleByIdQuery>
{
    public GetSampleByIdQueryValidator()
    {
        RuleFor(x => x.SampleId)
            .NotEmpty()
            .WithMessage("Sample identifier is required.");
    }
}

public sealed class GetSampleByCodeQueryValidator
    : AbstractValidator<GetSampleByCodeQuery>
{
    public GetSampleByCodeQueryValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .NotEmpty()
            .WithMessage("Laboratory identifier is required.");

        RuleFor(x => x.SampleCode)
            .NotEmpty()
            .WithMessage("Sample code is required.")
            .MaximumLength(50)
            .WithMessage("Sample code cannot exceed 50 characters.");
    }
}

public sealed class GetSamplesByLaboratoryIdQueryValidator
    : AbstractValidator<GetSamplesByLaboratoryIdQuery>
{
    public GetSamplesByLaboratoryIdQueryValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .NotEmpty()
            .WithMessage("Laboratory identifier is required.");
    }
}

public sealed class GetSamplesByAssignedUserIdQueryValidator
    : AbstractValidator<GetSamplesByAssignedUserIdQuery>
{
    public GetSamplesByAssignedUserIdQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User identifier is required.")
            .MaximumLength(150)
            .WithMessage("User identifier cannot exceed 150 characters.");
    }
}
