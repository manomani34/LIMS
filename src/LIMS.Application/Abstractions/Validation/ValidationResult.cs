namespace LIMS.Application.Abstractions.Validation;

public sealed class ValidationResult
{
    private readonly List<ValidationFailure> _errors = [];

    public IReadOnlyCollection<ValidationFailure> Errors => _errors.AsReadOnly();

    public bool IsValid => _errors.Count == 0;

    public bool IsInvalid => !IsValid;

    public ValidationResult() { }

    public ValidationResult(IEnumerable<ValidationFailure> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        _errors.AddRange(errors);
    }

    public void AddError(
        string propertyName,
        string errorMessage,
        string? errorCode = null)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException(
                "Error message cannot be empty.",
                nameof(errorMessage));

        _errors.Add(new ValidationFailure(
            propertyName,
            errorMessage,
            errorCode));
    }

    public void AddErrors(IEnumerable<ValidationFailure> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        _errors.AddRange(errors);
    }
}
