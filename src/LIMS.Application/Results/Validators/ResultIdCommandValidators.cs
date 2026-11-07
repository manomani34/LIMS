using LIMS.Application.Abstractions.Validation;

namespace LIMS.Application.Results.Validators;

public sealed class RemoveResultEntryCommandValidator
    : AbstractValidator<RemoveResultEntryCommand>
{
    public RemoveResultEntryCommandValidator()
    {
        RuleFor(x => x.ResultId).NotEmpty();
        RuleFor(x => x.EntryId).NotEmpty();
    }
}

public sealed class SubmitAnalysisResultCommandValidator
    : AbstractValidator<SubmitAnalysisResultCommand>
{
    public SubmitAnalysisResultCommandValidator()
    {
        RuleFor(x => x.ResultId).NotEmpty();

        RuleFor(x => x.SubmittedByUserId)
            .NotEmpty()
            .MaximumLength(150);
    }
}

public sealed class StartAnalysisResultReviewCommandValidator
    : AbstractValidator<StartAnalysisResultReviewCommand>
{
    public StartAnalysisResultReviewCommandValidator()
    {
        RuleFor(x => x.ResultId).NotEmpty();

        RuleFor(x => x.ReviewerUserId)
            .NotEmpty()
            .MaximumLength(150);
    }
}

public sealed class ApproveAnalysisResultCommandValidator
    : AbstractValidator<ApproveAnalysisResultCommand>
{
    public ApproveAnalysisResultCommandValidator()
    {
        RuleFor(x => x.ResultId).NotEmpty();

        RuleFor(x => x.ReviewerUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Comments)
            .MaximumLength(2000);
    }
}

public sealed class RejectAnalysisResultCommandValidator
    : AbstractValidator<RejectAnalysisResultCommand>
{
    public RejectAnalysisResultCommandValidator()
    {
        RuleFor(x => x.ResultId).NotEmpty();

        RuleFor(x => x.ReviewerUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(2000);
    }
}

public sealed class ReturnAnalysisResultToDraftCommandValidator
    : AbstractValidator<ReturnAnalysisResultToDraftCommand>
{
    public ReturnAnalysisResultToDraftCommandValidator()
    {
        RuleFor(x => x.ResultId).NotEmpty();

        RuleFor(x => x.RequestedByUserId)
            .NotEmpty()
            .MaximumLength(150);
    }
}

public sealed class ArchiveAnalysisResultCommandValidator
    : AbstractValidator<ArchiveAnalysisResultCommand>
{
    public ArchiveAnalysisResultCommandValidator()
    {
        RuleFor(x => x.ResultId).NotEmpty();

        RuleFor(x => x.ArchivedByUserId)
            .NotEmpty()
            .MaximumLength(150);
    }
}
