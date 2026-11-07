using LIMS.Application.Abstractions.Messaging;
using LIMS.Domain.Instruments.Enums;

namespace LIMS.Application.Instruments.Commands;

public sealed record UpdateInstrumentInformationCommand(
    Guid InstrumentId,
    string Name,
    InstrumentType Type,
    string? Manufacturer,
    string? Model,
    string? SerialNumber,
    string? Location,
    string? Description) : ICommand<bool>;
