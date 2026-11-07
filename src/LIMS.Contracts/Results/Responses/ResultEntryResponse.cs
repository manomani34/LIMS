using LIMS.Contracts.Results.Enums;

namespace LIMS.Contracts.Results.Responses;

public sealed record ResultEntryResponse(
    Guid Id,
    Guid ResultId,
    string ParameterName,
    ResultValueTypeContract ValueType,
    string? Value,
    decimal? NumericValue,
    string? Unit,
    decimal? Uncertainty,
    string? Method,
    string? Notes);
