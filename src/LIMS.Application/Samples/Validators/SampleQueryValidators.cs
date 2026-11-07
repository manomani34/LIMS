using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Samples.Queries;

namespace LIMS.Application.Samples.Validators;

public sealed class GetSampleByIdQueryValidator
    : AbstractValidator<GetSampleByIdQuery>
{
    public GetSampleByIdQueryValidator()
    {
        RuleFor(query => query.SampleId, (query, failures) =>
        {
            if (query.SampleId == Guid.Empty)
                failures.Add(new ValidationFailure(
                    nameof(query.SampleId),
                    "Sample identifier is required."));
        });
    }
}

public sealed class GetSampleByCodeQueryValidator
    : AbstractValidator<GetSampleByCodeQuery>
{
    public GetSampleByCodeQueryValidator()
    {
        RuleFor(query => query.LaboratoryId, (query, failures) =>
        {
            if (query.LaboratoryId == Guid.Empty)
                failures.Add(new ValidationFailure(
                    nameof(query.LaboratoryId),
                    "Laboratory identifier is required."));
        });

        RuleFor(query => query.SampleCode, (query, failures) =>
        {
            if (string.IsNullOrWhiteSpace(query.SampleCode))
                failures.Add(new ValidationFailure(
                    nameof(query.SampleCode),
                    "Sample code is required."));
            else if (query.SampleCode.Length > 50)
                failures.Add(new ValidationFailure(
                    nameof(query.SampleCode),
                    "Sample code cannot exceed 50 characters."));
        });
    }
}

public sealed class GetSamplesByLaboratoryIdQueryValidator
    : AbstractValidator<GetSamplesByLaboratoryIdQuery>
{
    public GetSamplesByLaboratoryIdQueryValidator()
    {
        RuleFor(query => query.LaboratoryId, (query, failures) =>
        {
            if (query.LaboratoryId == Guid.Empty)
                failures.Add(new ValidationFailure(
                    nameof(query.LaboratoryId),
                    "Laboratory identifier is required."));
        });
    }
}

public sealed class GetSamplesByAssignedUserIdQueryValidator
    : AbstractValidator<GetSamplesByAssignedUserIdQuery>
{
    public GetSamplesByAssignedUserIdQueryValidator()
    {
        RuleFor(query => query.UserId, (query, failures) =>
        {
            if (string.IsNullOrWhiteSpace(query.UserId))
                failures.Add(new ValidationFailure(
                    nameof(query.UserId),
                    "User identifier is required."));
        });
    }
}
