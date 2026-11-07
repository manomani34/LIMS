namespace LIMS.Contracts.Common;

public sealed record ErrorResponse(
    string Code,
    string Message,
    string? TraceId = null);
