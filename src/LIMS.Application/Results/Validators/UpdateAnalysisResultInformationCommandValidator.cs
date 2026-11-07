using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Results.Commands;
using LIMS.Application.Abstractions.Validation;

namespace LIMS.Application.Results.Validators;

public sealed class UpdateAnalysisResultInformationCommandValidator
    : AbstractValidator<UpdateAnalysisResultInformationCommand>
{
    public UpdateAnalysisResultInformationCommandValidator()
    {
        RuleFor(x => x.ResultId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(4000);

        RuleFor(x => x.InstrumentId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("InstrumentId must be a valid identifier when provided.");
    }
}
