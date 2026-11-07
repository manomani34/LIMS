using LIMS.Domain.Protocols.Enums;

namespace LIMS.Application.Protocols.DTOs;

public sealed record ProtocolStepDto(
    Guid Id,
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
    string? Notes);
