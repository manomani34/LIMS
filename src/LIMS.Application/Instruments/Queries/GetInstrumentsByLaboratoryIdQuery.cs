using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Instruments.Queries;

public sealed record GetInstrumentsByLaboratoryIdQuery(
    Guid LaboratoryId) : IQuery<IReadOnlyCollection<DTOs.InstrumentDto>>;
