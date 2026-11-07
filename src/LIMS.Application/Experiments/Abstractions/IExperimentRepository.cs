using LIMS.Application.Abstractions.Persistence;
using LIMS.Domain.Experiments;

namespace LIMS.Application.Experiments.Abstractions;

public interface IExperimentRepository : IRepository<Experiment, Guid>
{
    Task<bool> ExistsByCodeAsync(
        Guid laboratoryId,
        string experimentCode,
        CancellationToken cancellationToken = default);

    Task<Experiment?> GetWithDetailsAsync(
        Guid experimentId,
        CancellationToken cancellationToken = default);

    Task<Experiment?> GetByCodeAsync(
        Guid laboratoryId,
        string experimentCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Experiment>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Experiment>> GetBySampleIdAsync(
        Guid sampleId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Experiment>> GetByAssignedUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
