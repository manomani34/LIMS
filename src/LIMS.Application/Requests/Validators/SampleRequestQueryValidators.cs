using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Requests.Queries;

namespace LIMS.Application.Requests.Validators;

public sealed class GetSampleRequestByIdQueryValidator
    : AbstractValidator<GetSampleRequestByIdQuery>
{
    public GetSampleRequestByIdQueryValidator()
    {
        RuleFor(x => x.RequestId)
            .Must(x => x != Guid.Empty);
    }
}

public sealed class GetSampleRequestByCodeQueryValidator
    : AbstractValidator<GetSampleRequestByCodeQuery>
{
    public GetSampleRequestByCodeQueryValidator()
    {
        RuleFor(x => x.RequestCode)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public sealed class GetSampleRequestsByRequesterQueryValidator
    : AbstractValidator<GetSampleRequestsByRequesterQuery>
{
    public GetSampleRequestsByRequesterQueryValidator()
    {
        RuleFor(x => x.RequestedByUserId)
            .NotEmpty()
            .MaximumLength(150);
    }
}

public sealed class GetSampleRequestsByLaboratoryQueryValidator
    : AbstractValidator<GetSampleRequestsByLaboratoryQuery>
{
    public GetSampleRequestsByLaboratoryQueryValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .Must(x => x != Guid.Empty);
    }
}

public sealed class GetSampleRequestsByStatusQueryValidator
    : AbstractValidator<GetSampleRequestsByStatusQuery>
{
    public GetSampleRequestsByStatusQueryValidator()
    {
        RuleFor(x => x.Status)
            .Must(x => Enum.IsDefined(x));
    }
}
