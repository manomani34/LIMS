using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Samples.DTOs;
using LIMS.Application.Samples.Interfaces;
using LIMS.Application.Samples.Queries;

namespace LIMS.Application.Samples.Handlers;

public sealed class GetSamplesByAssignedUserIdQueryHandler
    : IQueryHandler<GetSamplesByAssignedUserIdQuery, IReadOnlyCollection<SampleDto>>
{
    private readonly ISampleRepository _sampleRepository;

    public GetSamplesByAssignedUserIdQueryHandler(ISampleRepository sampleRepository)
    {
        _sampleRepository = sampleRepository;
    }

    public async Task<IReadOnlyCollection<SampleDto>> HandleAsync(
        GetSamplesByAssignedUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var samples = await _sampleRepository.GetByAssignedUserIdAsync(
            query.UserId,
            cancellationToken);

        return samples.Select(sample => new SampleDto(
            sample.Id,
            sample.LaboratoryId,
            sample.SampleCode,
            sample.Name,
            sample.Description,
            sample.Type,
            sample.Status,
            sample.Priority,
            sample.Source,
            sample.AssignedToUserId,
            sample.RegisteredByUserId,
            sample.RegisteredAt,
            sample.ReceivedAt,
            sample.UpdatedAt,
            sample.ArchivedAt)).ToArray();
    }
}
