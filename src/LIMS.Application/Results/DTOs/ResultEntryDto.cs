using LIMS.Domain.Results.Enums;

namespace LIMS.Application.Results.DTOs;

public sealed record ResultEntryDto(
    Guid Id,
    Guid ResultId,
    string ParameterName,
    ResultValueType ValueType,
    string? Value,
    decimal? NumericValue,
    string? Unit,
    decimal? Uncertainty,
    string? Method,
    string? Notes);
