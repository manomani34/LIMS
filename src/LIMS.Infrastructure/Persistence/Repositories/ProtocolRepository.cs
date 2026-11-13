
using LIMS.Application.Protocols.Abstractions;
using LIMS.Domain.Protocols;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Infrastructure.Persistence.Repositories;

public sealed class ProtocolRepository
    : GenericRepository<Protocol, Guid>, IProtocolRepository
{
    public ProtocolRepository(LimsDbContext context)
        : base(context)
    {
    }

    public Task<bool> ExistsByCodeAsync(
        Guid laboratoryId,
        string protocolCode,
        CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(
            x => x.LaboratoryId == laboratoryId
                 && x.ProtocolCode == protocolCode,
            cancellationToken);
    }

    public Task<Protocol?> GetWithDetailsAsync(
        Guid protocolId,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(x => x.Steps)
            .FirstOrDefaultAsync(
                x => x.Id == protocolId,
                cancellationToken);
    }

    public Task<Protocol?> GetByCodeAsync(
        Guid laboratoryId,
        string protocolCode,
        CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(
            x => x.LaboratoryId == laboratoryId
                 && x.ProtocolCode == protocolCode,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Protocol>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.LaboratoryId == laboratoryId)
            .OrderBy(x => x.ProtocolCode)
            .ThenByDescending(x => x.Version)
            .ToListAsync(cancellationToken);
    }
}
