
using LIMS.Application.Abstractions.Persistence;

namespace LIMS.UnitTests.Application.Inventory.Fakes;

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCallCount { get; private set; }

    public CancellationToken LastCancellationToken { get; private set; }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        SaveChangesCallCount++;
        LastCancellationToken = cancellationToken;

        return Task.FromResult(1);
    }
}
