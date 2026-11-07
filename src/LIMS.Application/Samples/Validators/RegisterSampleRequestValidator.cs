using LIMS.Contracts.Samples;

namespace LIMS.Application.Samples.Validators;

public static class SampleContractValidation
{
    public static bool IsValid(RegisterSampleRequest request)
    {
        return request.LaboratoryId != Guid.Empty
            && !string.IsNullOrWhiteSpace(request.SampleCode)
            && request.SampleCode.Length <= 50
            && !string.IsNullOrWhiteSpace(request.Name)
            && request.Name.Length <= 200
            && (request.Description?.Length ?? 0) <= 2000
            && (request.Source?.Length ?? 0) <= 250
            && Enum.IsDefined(request.Type)
            && Enum.IsDefined(request.Priority)
            && !string.IsNullOrWhiteSpace(request.RegisteredByUserId);
    }
}
