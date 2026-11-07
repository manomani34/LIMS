namespace LIMS.Application.Abstractions.Validation;

public sealed record ValidationFailure(
    string PropertyName,
    string ErrorMessage,
    string? ErrorCode = null);
