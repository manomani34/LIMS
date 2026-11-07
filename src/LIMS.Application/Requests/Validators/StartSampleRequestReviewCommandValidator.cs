using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Requests.Commands;
using LIMS.Application.Abstractions.Validation;

namespace LIMS.Application.Requests.Validators;

public sealed class StartSampleRequestReviewCommandValidator
    : AbstractValidator<StartSampleRequestReviewCommand>
{
    public StartSampleRequestReviewCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .Must(x => x != Guid.Empty);

        RuleFor(x => x.ReviewerUserId)
            .NotEmpty()
            .MaximumLength(150);
    }
}
