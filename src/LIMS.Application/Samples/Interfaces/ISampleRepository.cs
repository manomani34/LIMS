using LIMS.Application.Abstractions.Persistence;
using LIMS.Domain.Samples;

namespace LIMS.Application.Samples.Interfaces;

public interface ISampleRepository : IRepository<Sample, Guid>
{
    Task<bool> ExistsByCodeAsync(
        Guid laboratoryId,
        string sampleCode,
        CancellationToken cancellationToken = default);

    Task<Sample?> GetByCodeAsync(
        Guid laboratoryId,
        string sampleCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Sample>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Sample>> GetByAssignedUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Sample?> GetWithDetailsAsync(
        Guid sampleId,
        CancellationToken cancellationToken = default);
}
