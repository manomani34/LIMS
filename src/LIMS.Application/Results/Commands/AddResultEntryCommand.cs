using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Common.Dispatching;
using LIMS.Domain.Results.Enums;

namespace LIMS.Application.Results.Commands;

public sealed record AddResultEntryCommand(
    Guid ResultId,
    string ParameterName,
    ResultValueType ValueType,
    string? Value,
    decimal? NumericValue,
    string? Unit,
    decimal? Uncertainty,
    string? Method,
    string? Notes) : ICommand<Guid>;
