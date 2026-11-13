
using LIMS.Application.Experiments.Abstractions;
using LIMS.Domain.Experiments;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Infrastructure.Persistence.Repositories;

public sealed class ExperimentRepository
    : GenericRepository<Experiment, Guid>, IExperimentRepository
{
    public ExperimentRepository(LimsDbContext context)
        : base(context)
    {
    }

    public Task<bool> ExistsByCodeAsync(
        Guid laboratoryId,
        string experimentCode,
        CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(
            x => x.LaboratoryId == laboratoryId
                 && x.ExperimentCode == experimentCode,
            cancellationToken);
    }

    public Task<Experiment?> GetWithDetailsAsync(
        Guid experimentId,
        CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(
            x => x.Id == experimentId,
            cancellationToken);
    }

    public Task<Experiment?> GetByCodeAsync(
        Guid laboratoryId,
        string experimentCode,
        CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(
            x => x.LaboratoryId == laboratoryId
                 && x.ExperimentCode == experimentCode,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Experiment>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.LaboratoryId == laboratoryId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Experiment>> GetBySampleIdAsync(
        Guid sampleId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.SampleId == sampleId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Experiment>> GetByAssignedUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.AssignedToUserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
