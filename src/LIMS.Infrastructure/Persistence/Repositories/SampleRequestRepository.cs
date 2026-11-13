
using LIMS.Application.Requests.Repositories;
using LIMS.Domain.Requests;
using LIMS.Domain.Requests.Enums;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Infrastructure.Persistence.Repositories;

public sealed class SampleRequestRepository
    : GenericRepository<SampleRequest, Guid>, ISampleRequestRepository
{
    public SampleRequestRepository(LimsDbContext context)
        : base(context)
    {
    }

    public Task<bool> ExistsByCodeAsync(
        string requestCode,
        CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(
            x => x.RequestCode == requestCode,
            cancellationToken);
    }

    public Task<SampleRequest?> GetByCodeAsync(
        string requestCode,
        CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(
            x => x.RequestCode == requestCode,
            cancellationToken);
    }

    public Task<SampleRequest?> GetWithDetailsAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(
            x => x.Id == requestId,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<SampleRequest>> GetByRequesterAsync(
        string requestedByUserId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.RequestedByUserId == requestedByUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SampleRequest>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.LaboratoryId == laboratoryId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SampleRequest>> GetByStatusAsync(
        SampleRequestStatus status,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SampleRequest>> GetPendingReviewAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.Status == SampleRequestStatus.UnderReview)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
