using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Requests.Commands;

namespace LIMS.Application.Requests.Validators;

public sealed class FulfillSampleRequestCommandValidator
    : AbstractValidator<FulfillSampleRequestCommand>
{
    public FulfillSampleRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .Must(x => x != Guid.Empty);

        RuleFor(x => x.SampleId)
            .Must(x => x != Guid.Empty);
    }
}
