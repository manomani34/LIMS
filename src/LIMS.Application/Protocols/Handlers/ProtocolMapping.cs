using LIMS.Application.Protocols.DTOs;
using LIMS.Domain.Protocols;

namespace LIMS.Application.Protocols.Handlers;

internal static class ProtocolMapping
{
    public static ProtocolDto ToDto(this Protocol protocol)
    {
        return new ProtocolDto(
            protocol.Id,
            protocol.LaboratoryId,
            protocol.ProtocolCode,
            protocol.Name,
            protocol.Description,
            protocol.Version,
            protocol.Status,
            protocol.CreatedByUserId,
            protocol.CreatedAt,
            protocol.UpdatedAt,
            protocol.ActivatedAt,
            protocol.DeprecatedAt,
            protocol.ArchivedAt);
    }

    public static ProtocolDetailsDto ToDetailsDto(this Protocol protocol)
    {
        return new ProtocolDetailsDto(
            protocol.Id,
            protocol.LaboratoryId,
            protocol.ProtocolCode,
            protocol.Name,
            protocol.Description,
            protocol.Version,
            protocol.Status,
            protocol.CreatedByUserId,
            protocol.CreatedAt,
            protocol.UpdatedAt,
            protocol.ActivatedAt,
            protocol.DeprecatedAt,
            protocol.ArchivedAt,
            protocol.Steps
                .OrderBy(x => x.Sequence)
                .Select(x => new ProtocolStepDto(
                    x.Id,
                    x.ProtocolId,
                    x.Sequence,
                    x.Title,
                    x.Instruction,
                    x.Type,
                    x.IsRequired,
                    x.TargetValue,
                    x.TargetUnit,
                    x.MinimumValue,
                    x.MaximumValue,
                    x.Notes))
                .ToArray());
    }
}
