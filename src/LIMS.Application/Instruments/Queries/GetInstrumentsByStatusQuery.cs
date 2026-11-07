using LIMS.Application.Abstractions.Messaging;
using LIMS.Domain.Instruments.Enums;

namespace LIMS.Application.Instruments.Queries;

public sealed record GetInstrumentsByStatusQuery(
    Guid LaboratoryId,
    InstrumentStatus Status) : IQuery<IReadOnlyCollection<DTOs.InstrumentDto>>;
