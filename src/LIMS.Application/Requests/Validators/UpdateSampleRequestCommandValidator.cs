using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Requests.Commands;
using LIMS.Application.Abstractions.Validation;

namespace LIMS.Application.Requests.Validators;

public sealed class UpdateSampleRequestCommandValidator
    : AbstractValidator<UpdateSampleRequestCommand>
{
    public UpdateSampleRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .Must(x => x != Guid.Empty);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(4000);

        RuleFor(x => x.SampleName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.SampleDescription)
            .MaximumLength(2000);

        RuleFor(x => x.SampleSource)
            .MaximumLength(250);

        RuleFor(x => x.RequestedSampleType)
            .Must(x => Enum.IsDefined(x));

        RuleFor(x => x.Priority)
            .Must(x => Enum.IsDefined(x));
    }
}
