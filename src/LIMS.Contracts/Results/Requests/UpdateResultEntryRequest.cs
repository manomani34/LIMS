using LIMS.Contracts.Results.Enums;

namespace LIMS.Contracts.Results.Requests;

public sealed record UpdateResultEntryRequest(
    string ParameterName,
    ResultValueTypeContract ValueType,
    string? Value,
    decimal? NumericValue,
    string? Unit,
    decimal? Uncertainty,
    string? Method,
    string? Notes);
