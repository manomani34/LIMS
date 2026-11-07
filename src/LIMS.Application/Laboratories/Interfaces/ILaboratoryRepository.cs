using LIMS.Application.Abstractions.Persistence;
using LIMS.Domain.Laboratories;

namespace LIMS.Application.Laboratories.Interfaces;

public interface ILaboratoryRepository : IRepository<Laboratory, Guid>
{
    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<Laboratory?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<Laboratory?> GetWithMembersAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Laboratory>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
