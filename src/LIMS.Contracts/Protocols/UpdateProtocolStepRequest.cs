namespace LIMS.Contracts.Protocols;

public sealed record UpdateProtocolStepRequest(
    int Sequence,
    string Title,
    string Instruction,
    ProtocolStepTypeContract Type,
    bool IsRequired,
    decimal? TargetValue,
    string? TargetUnit,
    decimal? MinimumValue,
    decimal? MaximumValue,
    string? Notes);
