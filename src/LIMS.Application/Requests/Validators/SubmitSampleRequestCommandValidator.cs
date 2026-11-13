using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Requests.Commands;

namespace LIMS.Application.Requests.Validators;

public sealed class SubmitSampleRequestCommandValidator
    : AbstractValidator<SubmitSampleRequestCommand>
{
    public SubmitSampleRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .Must(x => x != Guid.Empty);

        RuleFor(x => x.SubmittedByUserId)
            .NotEmpty()
            .MaximumLength(150);
    }
}
