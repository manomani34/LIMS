namespace LIMS.Application.Abstractions.Validation;

public sealed class ValidationException : Exception
{
    public IReadOnlyCollection<ValidationFailure> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> errors)
        : base("One or more validation errors occurred.")
    {
        ArgumentNullException.ThrowIfNull(errors);

        Errors = errors.ToArray();

        if (Errors.Count == 0)
            throw new ArgumentException(
                "At least one validation error is required.",
                nameof(errors));
    }
}
