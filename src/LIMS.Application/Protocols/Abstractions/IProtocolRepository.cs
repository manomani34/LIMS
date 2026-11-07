using LIMS.Application.Abstractions.Persistence;
using LIMS.Domain.Protocols;

namespace LIMS.Application.Protocols.Abstractions;

public interface IProtocolRepository : IRepository<Protocol, Guid>
{
    Task<bool> ExistsByCodeAsync(
        Guid laboratoryId,
        string protocolCode,
        CancellationToken cancellationToken = default);

    Task<Protocol?> GetWithDetailsAsync(
        Guid protocolId,
        CancellationToken cancellationToken = default);

    Task<Protocol?> GetByCodeAsync(
        Guid laboratoryId,
        string protocolCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Protocol>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default);
}
