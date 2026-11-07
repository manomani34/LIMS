using LIMS.Application.Files.Queries;
using LIMS.Application.Abstractions.Validation;

namespace LIMS.Application.Files.Validators;

public sealed class GetFileByIdQueryValidator
    : AbstractValidator<GetFileByIdQuery>
{
    public GetFileByIdQueryValidator()
    {
        RuleFor(x => x.FileId)
            .Must(x => x != Guid.Empty)
            .WithMessage("File ID is required.");
    }
}

public sealed class GetFilesByReferenceQueryValidator
    : AbstractValidator<GetFilesByReferenceQuery>
{
    public GetFilesByReferenceQueryValidator()
    {
        RuleFor(x => x.ReferenceType)
            .Must(x => Enum.IsDefined(x))
            .WithMessage("Reference type is invalid.");

        RuleFor(x => x.ReferenceId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Reference ID is required.");
    }
}

public sealed class GetFileReferencesQueryValidator
    : AbstractValidator<GetFileReferencesQuery>
{
    public GetFileReferencesQueryValidator()
    {
        RuleFor(x => x.FileId)
            .Must(x => x != Guid.Empty)
            .WithMessage("File ID is required.");
    }
}
