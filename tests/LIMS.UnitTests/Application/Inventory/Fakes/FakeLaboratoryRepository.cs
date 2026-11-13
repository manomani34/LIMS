
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Laboratories.Interfaces;
using LIMS.Domain.Laboratories;

namespace LIMS.UnitTests.Application.Inventory.Fakes;

public sealed class FakeLaboratoryRepository : ILaboratoryRepository
{
    private readonly List<Laboratory> _laboratories = [];

    public int AddCallCount { get; private set; }

    public int UpdateCallCount { get; private set; }

    public int RemoveCallCount { get; private set; }

    public void Seed(params Laboratory[] laboratories)
    {
        _laboratories.AddRange(laboratories);
    }

    public Task<Laboratory?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _laboratories.FirstOrDefault(x => x.Id == id));
    }

    public Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _laboratories.Any(x =>
                string.Equals(
                    x.Code,
                    code,
                    StringComparison.OrdinalIgnoreCase)));
    }

    public Task<Laboratory?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _laboratories.FirstOrDefault(x =>
                string.Equals(
                    x.Code,
                    code,
                    StringComparison.OrdinalIgnoreCase)));
    }

    public Task<Laboratory?> GetWithMembersAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default)
    {
        return GetByIdAsync(laboratoryId, cancellationToken);
    }

    public Task<IReadOnlyCollection<Laboratory>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Laboratory> result = _laboratories
            .Where(x => x.Members.Any(m =>
                string.Equals(
                    m.UserId,
                    userId,
                    StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        return Task.FromResult(result);
    }

    public Task AddAsync(
        Laboratory entity,
        CancellationToken cancellationToken = default)
    {
        AddCallCount++;
        _laboratories.Add(entity);

        return Task.CompletedTask;
    }

    public void Update(Laboratory entity)
    {
        UpdateCallCount++;

        var index = _laboratories.FindIndex(x => x.Id == entity.Id);

        if (index >= 0)
            _laboratories[index] = entity;
        else
            _laboratories.Add(entity);
    }

    public void Remove(Laboratory entity)
    {
        RemoveCallCount++;
        _laboratories.RemoveAll(x => x.Id == entity.Id);
    }
}
