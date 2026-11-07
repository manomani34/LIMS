using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Laboratories.DTOs;
using LIMS.Application.Laboratories.Interfaces;
using LIMS.Application.Laboratories.Queries;

namespace LIMS.Application.Laboratories.Handlers;

public sealed class GetLaboratoryByCodeQueryHandler
    : IQueryHandler<GetLaboratoryByCodeQuery, LaboratoryDto?>
{
    private readonly ILaboratoryRepository _laboratoryRepository;

    public GetLaboratoryByCodeQueryHandler(
        ILaboratoryRepository laboratoryRepository)
    {
        _laboratoryRepository = laboratoryRepository;
    }

    public async Task<LaboratoryDto?> HandleAsync(
        GetLaboratoryByCodeQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (string.IsNullOrWhiteSpace(query.Code))
            throw new ArgumentException("Laboratory code is required.");

        var laboratory = await _laboratoryRepository.GetByCodeAsync(
            query.Code.Trim(),
            cancellationToken);

        if (laboratory is null)
            return null;

        return new LaboratoryDto(
            laboratory.Id,
            laboratory.Name,
            laboratory.Code,
            laboratory.Description,
            laboratory.Status,
            laboratory.CreatedAt,
            laboratory.UpdatedAt,
            laboratory.Members.Count);
    }
}
