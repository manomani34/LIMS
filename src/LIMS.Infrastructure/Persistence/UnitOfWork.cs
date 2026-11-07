using LIMS.Application.Abstractions.Persistence;

namespace LIMS.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException(
            "Persistence provider has not been configured yet.");
    }
}
