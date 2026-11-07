using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Laboratories.DTOs;
using LIMS.Application.Laboratories.Interfaces;
using LIMS.Application.Laboratories.Queries;

namespace LIMS.Application.Laboratories.Handlers;

public sealed class GetLaboratoryByIdQueryHandler
    : IQueryHandler<GetLaboratoryByIdQuery, LaboratoryDetailsDto?>
{
    private readonly ILaboratoryRepository _laboratoryRepository;

    public GetLaboratoryByIdQueryHandler(
        ILaboratoryRepository laboratoryRepository)
    {
        _laboratoryRepository = laboratoryRepository;
    }

    public async Task<LaboratoryDetailsDto?> HandleAsync(
        GetLaboratoryByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.LaboratoryId == Guid.Empty)
            throw new ArgumentException("Laboratory identifier is required.");

        var laboratory = await _laboratoryRepository.GetWithMembersAsync(
            query.LaboratoryId,
            cancellationToken);

        if (laboratory is null)
            return null;

        var members = laboratory.Members
            .Select(member => new LaboratoryMemberDto(
                member.Id,
                member.UserId,
                member.Role,
                member.JoinedAt))
            .ToArray();

        return new LaboratoryDetailsDto(
            laboratory.Id,
            laboratory.Name,
            laboratory.Code,
            laboratory.Description,
            laboratory.Status,
            laboratory.CreatedAt,
            laboratory.UpdatedAt,
            members);
    }
}
