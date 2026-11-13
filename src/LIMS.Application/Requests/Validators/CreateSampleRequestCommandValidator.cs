using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Requests.Commands;

namespace LIMS.Application.Requests.Validators;

public sealed class CreateSampleRequestCommandValidator
    : AbstractValidator<CreateSampleRequestCommand>
{
    public CreateSampleRequestCommandValidator()
    {
        RuleFor(x => x.RequestCode)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[A-Za-z0-9_-]+$");

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

        RuleFor(x => x.RequestedByUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.RequestedSampleType)
            .Must(x => Enum.IsDefined(x));

        RuleFor(x => x.Priority)
            .Must(x => Enum.IsDefined(x));
    }
}
