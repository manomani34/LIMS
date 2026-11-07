using LIMS.Domain.Instruments.Enums;

namespace LIMS.Application.Instruments.DTOs;

public sealed record InstrumentDetailsDto(
    Guid Id,
    Guid LaboratoryId,
    string InstrumentCode,
    string Name,
    InstrumentType Type,
    InstrumentStatus Status,
    string? Manufacturer,
    string? Model,
    string? SerialNumber,
    string? Location,
    string? Description,
    DateTimeOffset RegisteredAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? RetiredAt,
    IReadOnlyCollection<InstrumentMaintenanceDto> MaintenanceRecords);
