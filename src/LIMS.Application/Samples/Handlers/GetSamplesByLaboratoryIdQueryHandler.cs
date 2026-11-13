using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Samples.DTOs;
using LIMS.Application.Samples.Interfaces;
using LIMS.Application.Samples.Queries;

namespace LIMS.Application.Samples.Handlers;

public sealed class GetSamplesByLaboratoryIdQueryHandler
    : IQueryHandler<GetSamplesByLaboratoryIdQuery, IReadOnlyCollection<SampleDto>>
{
    private readonly ISampleRepository _sampleRepository;

    public GetSamplesByLaboratoryIdQueryHandler(ISampleRepository sampleRepository)
    {
        _sampleRepository = sampleRepository;
    }

    public async Task<IReadOnlyCollection<SampleDto>> HandleAsync(
        GetSamplesByLaboratoryIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var samples = await _sampleRepository.GetByLaboratoryIdAsync(
            query.LaboratoryId,
            cancellationToken);

        return samples.Select(sample => new SampleDto(
            sample.Id,
            sample.LaboratoryId,
            sample.SampleCode,
            sample.Name,
            sample.Description,
            sample.RejectionReason,
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
