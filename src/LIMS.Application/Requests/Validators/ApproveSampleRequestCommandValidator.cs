using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Requests.Commands;

namespace LIMS.Application.Requests.Validators;

public sealed class ApproveSampleRequestCommandValidator
    : AbstractValidator<ApproveSampleRequestCommand>
{
    public ApproveSampleRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .Must(x => x != Guid.Empty);

        RuleFor(x => x.ApprovedByUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.LaboratoryId)
            .Must(x => x != Guid.Empty);

        RuleFor(x => x.Comments)
            .MaximumLength(2000);
    }
}
