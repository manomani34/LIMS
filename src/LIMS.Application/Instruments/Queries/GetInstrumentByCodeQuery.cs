using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Instruments.Queries;

public sealed record GetInstrumentByCodeQuery(
    Guid LaboratoryId,
    string InstrumentCode) : IQuery<DTOs.InstrumentDto?>;
