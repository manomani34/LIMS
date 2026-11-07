namespace LIMS.Contracts.Instruments.Requests;

public sealed record RegisterInstrumentRequest(
    Guid LaboratoryId,
    string InstrumentCode,
    string Name,
    InstrumentTypeContract Type,
    string RegisteredByUserId,
    string? Manufacturer,
    string? Model,
    string? SerialNumber,
    string? Location,
    string? Description);
