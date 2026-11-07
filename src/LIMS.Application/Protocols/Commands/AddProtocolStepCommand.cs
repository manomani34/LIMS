using LIMS.Application.Abstractions.Messaging;
using LIMS.Domain.Protocols.Enums;

namespace LIMS.Application.Protocols.Commands;

public sealed record AddProtocolStepCommand(
    Guid ProtocolId,
    int Sequence,
    string Title,
    string Instruction,
    ProtocolStepType Type,
    bool IsRequired,
    decimal? TargetValue,
    string? TargetUnit,
    decimal? MinimumValue,
    decimal? MaximumValue,
    string? Notes) : ICommand<Guid>;
