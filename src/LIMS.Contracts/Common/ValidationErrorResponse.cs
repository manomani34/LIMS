namespace LIMS.Contracts.Common;

public sealed record ValidationError(
    string Field,
    string Message,
    string? Code = null);

public sealed record ValidationErrorResponse(
    string Code,
    string Message,
    IReadOnlyCollection<ValidationError> Errors,
    string? TraceId = null);
