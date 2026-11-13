using LIMS.Application.Files.Commands;
using LIMS.Application.Abstractions.Validation;

namespace LIMS.Application.Files.Validators;

public sealed class RegisterFileCommandValidator
    : AbstractValidator<RegisterFileCommand>
{
    public RegisterFileCommandValidator()
    {
        RuleFor(x => x.OriginalFileName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.StorageKey)
    .NotEmpty()
    .MaximumLength(1000);

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.FileExtension)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.SizeInBytes)
            .Must(x => x > 0)
            .WithMessage("File size must be greater than zero.");

        RuleFor(x => x.Sha256Checksum)
     .NotEmpty()
     .Must(x => x is { Length: 64 } && x.All(Uri.IsHexDigit))
     .WithMessage("A valid SHA-256 checksum is required.");

        RuleFor(x => x.Category)
            .Must(x => Enum.IsDefined(x))
            .WithMessage("File category is invalid.");

        RuleFor(x => x.UploadedByUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description).MaximumLength(2000);
    }
}

public sealed class FileIdCommandValidator<TCommand>
    : AbstractValidator<TCommand>
    where TCommand : class
{
    public FileIdCommandValidator(
        System.Linq.Expressions.Expression<Func<TCommand, Guid>> idExpression)
    {
        RuleFor(idExpression)
            .Must(x => x != Guid.Empty)
            .WithMessage("File ID is required.");
    }
}

public sealed class QuarantineFileCommandValidator
    : AbstractValidator<QuarantineFileCommand>
{
    public QuarantineFileCommandValidator()
    {
        RuleFor(x => x.FileId)
            .Must(x => x != Guid.Empty)
            .WithMessage("File ID is required.");

        RuleFor(x => x.Reason)
    .NotEmpty()
    .MaximumLength(1000);
    }
}

public sealed class RejectFileCommandValidator
    : AbstractValidator<RejectFileCommand>
{
    public RejectFileCommandValidator()
    {
        RuleFor(x => x.FileId)
            .Must(x => x != Guid.Empty)
            .WithMessage("File ID is required.");

        RuleFor(x => x.Reason)
    .NotEmpty()
    .MaximumLength(1000);
    }
}

public sealed class DeleteFileCommandValidator
    : AbstractValidator<DeleteFileCommand>
{
    public DeleteFileCommandValidator()
    {
        RuleFor(x => x.FileId)
            .Must(x => x != Guid.Empty)
            .WithMessage("File ID is required.");

        RuleFor(x => x.DeletedByUserId)
            .NotEmpty()
            .MaximumLength(150);
    }
}

public sealed class LinkFileCommandValidator
    : AbstractValidator<LinkFileCommand>
{
    public LinkFileCommandValidator()
    {
        RuleFor(x => x.FileId)
            .Must(x => x != Guid.Empty)
            .WithMessage("File ID is required.");

        RuleFor(x => x.ReferenceType)
            .Must(x => Enum.IsDefined(x))
            .WithMessage("Reference type is invalid.");

        RuleFor(x => x.ReferenceId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Reference ID is required.");

        RuleFor(x => x.LinkedByUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Purpose).MaximumLength(500);
    }
}

public sealed class UnlinkFileCommandValidator
    : AbstractValidator<UnlinkFileCommand>
{
    public UnlinkFileCommandValidator()
    {
        RuleFor(x => x.FileReferenceId)
            .Must(x => x != Guid.Empty)
            .WithMessage("File reference ID is required.");
    }
}
