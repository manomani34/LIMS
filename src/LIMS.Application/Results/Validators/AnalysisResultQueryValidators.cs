using LIMS.Application.Results.Commands;
using LIMS.Application.Results.Queries;
using LIMS.Application.Abstractions.Validation;
using LIMS.Domain.Results.Enums;

namespace LIMS.Application.Results.Validators;

public sealed class GetAnalysisResultByIdQueryValidator
    : AbstractValidator<GetAnalysisResultByIdQuery>
{
    public GetAnalysisResultByIdQueryValidator()
    {
        RuleFor(x => x.ResultId).NotEmpty();
    }
}

public sealed class GetAnalysisResultByCodeQueryValidator
    : AbstractValidator<GetAnalysisResultByCodeQuery>
{
    public GetAnalysisResultByCodeQueryValidator()
    {
        RuleFor(x => x.LaboratoryId).NotEmpty();

        RuleFor(x => x.ResultCode)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public sealed class GetAnalysisResultsByLaboratoryIdQueryValidator
    : AbstractValidator<GetAnalysisResultsByLaboratoryIdQuery>
{
    public GetAnalysisResultsByLaboratoryIdQueryValidator()
    {
        RuleFor(x => x.LaboratoryId).NotEmpty();
    }
}

public sealed class GetAnalysisResultsByExperimentIdQueryValidator
    : AbstractValidator<GetAnalysisResultsByExperimentIdQuery>
{
    public GetAnalysisResultsByExperimentIdQueryValidator()
    {
        RuleFor(x => x.ExperimentId).NotEmpty();
    }
}

public sealed class GetAnalysisResultsBySampleIdQueryValidator
    : AbstractValidator<GetAnalysisResultsBySampleIdQuery>
{
    public GetAnalysisResultsBySampleIdQueryValidator()
    {
        RuleFor(x => x.SampleId).NotEmpty();
    }
}

public sealed class GetAnalysisResultsByStatusQueryValidator
    : AbstractValidator<GetAnalysisResultsByStatusQuery>
{
    public GetAnalysisResultsByStatusQueryValidator()
    {
        RuleFor(x => x.LaboratoryId).NotEmpty();

        RuleFor(x => x.Status)
            .Must(Enum.IsDefined)
            .WithMessage("Invalid result status.");
    }
}
