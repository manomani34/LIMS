namespace LIMS.Contracts.Instruments.Requests;

public sealed record UpdateInstrumentRequest(
    string Name,
    InstrumentTypeContract Type,
    string? Manufacturer,
    string? Model,
    string? SerialNumber,
    string? Location,
    string? Description);
