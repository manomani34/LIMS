using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Instruments.Queries;

public sealed record GetInstrumentByIdQuery(
    Guid InstrumentId) : IQuery<DTOs.InstrumentDetailsDto?>;
