namespace LIMS.Contracts.Instruments.Responses;

public sealed record InstrumentResponse(
    Guid Id,
    Guid LaboratoryId,
    string InstrumentCode,
    string Name,
    string? Manufacturer,
    string? Model,
    string? SerialNumber,
    InstrumentTypeContract Type,
    InstrumentStatusContract Status,
    string? Location,
    string? Description,
    DateTimeOffset RegisteredAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? RetiredAt);
