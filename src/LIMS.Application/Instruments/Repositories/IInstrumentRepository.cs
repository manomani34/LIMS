using LIMS.Application.Abstractions.Persistence;
using LIMS.Domain.Instruments;
using LIMS.Domain.Instruments.Enums;

namespace LIMS.Application.Instruments.Repositories;

public interface IInstrumentRepository : IRepository<Instrument, Guid>
{
    Task<bool> ExistsByCodeAsync(
        Guid laboratoryId,
        string instrumentCode,
        CancellationToken cancellationToken = default);

    Task<Instrument?> GetWithDetailsAsync(
        Guid instrumentId,
        CancellationToken cancellationToken = default);

    Task<Instrument?> GetByCodeAsync(
        Guid laboratoryId,
        string instrumentCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Instrument>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Instrument>> GetByStatusAsync(
        Guid laboratoryId,
        InstrumentStatus status,
        CancellationToken cancellationToken = default);
}
