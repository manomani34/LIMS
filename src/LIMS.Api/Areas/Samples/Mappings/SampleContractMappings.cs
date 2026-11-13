
using LIMS.Application.Samples.DTOs;
using LIMS.Contracts.Samples;
using LIMS.Contracts.Samples.Responses;
using LIMS.Domain.Samples.Enums;

namespace LIMS.Api.Areas.Samples.Mappings;

public static class SampleContractMappings
{
    public static SampleType ToDomain(this SampleTypeContract value)
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Invalid sample type.");
        }

        return (SampleType)(int)value;
    }

    public static SamplePriority ToDomain(this SamplePriorityContract value)
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Invalid sample priority.");
        }

        return (SamplePriority)(int)value;
    }

    public static SampleResponse ToResponse(this SampleDto dto)
    {
        return new SampleResponse(
            dto.Id,
            dto.LaboratoryId,
            dto.SampleCode,
            dto.Name,
            dto.Description,
            dto.RejectionReason,
            dto.Type.ToString(),
            dto.Status.ToString(),
            dto.Priority.ToString(),
            dto.Source,
            dto.AssignedToUserId,
            dto.RegisteredByUserId,
            dto.RegisteredAt,
            dto.ReceivedAt,
            dto.UpdatedAt,
            dto.ArchivedAt);
    }

    public static SampleResponse ToResponse(this SampleDetailsDto dto)
    {
        return new SampleResponse(
            dto.Id,
            dto.LaboratoryId,
            dto.SampleCode,
            dto.Name,
            dto.Description,
            dto.RejectionReason,
            dto.Type.ToString(),
            dto.Status.ToString(),
            dto.Priority.ToString(),
            dto.Source,
            dto.AssignedToUserId,
            dto.RegisteredByUserId,
            dto.RegisteredAt,
            dto.ReceivedAt,
            dto.UpdatedAt,
            dto.ArchivedAt);
    }
}
