using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Requests.Commands;
using LIMS.Application.Abstractions.Validation;

namespace LIMS.Application.Requests.Validators;

public sealed class CancelSampleRequestCommandValidator
    : AbstractValidator<CancelSampleRequestCommand>
{
    public CancelSampleRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .Must(x => x != Guid.Empty);

        RuleFor(x => x.CancelledByUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Reason)
            .MaximumLength(2000);
    }
}
