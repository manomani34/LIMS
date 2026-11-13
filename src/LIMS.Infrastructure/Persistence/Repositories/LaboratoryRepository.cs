
using LIMS.Application.Laboratories.Interfaces;
using LIMS.Domain.Laboratories;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Infrastructure.Persistence.Repositories;

public sealed class LaboratoryRepository
    : GenericRepository<Laboratory, Guid>, ILaboratoryRepository
{
    public LaboratoryRepository(LimsDbContext context)
        : base(context)
    {
    }

    public Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(
            x => x.Code == code,
            cancellationToken);
    }

    public Task<Laboratory?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(
            x => x.Code == code,
            cancellationToken);
    }

    public Task<Laboratory?> GetWithMembersAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(x => x.Members)
            .FirstOrDefaultAsync(
                x => x.Id == laboratoryId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Laboratory>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.Members.Any(member => member.UserId == userId))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
