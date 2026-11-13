using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Requests.Commands;

namespace LIMS.Application.Requests.Validators;

public sealed class RejectSampleRequestCommandValidator
    : AbstractValidator<RejectSampleRequestCommand>
{
    public RejectSampleRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .Must(x => x != Guid.Empty);

        RuleFor(x => x.RejectedByUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
