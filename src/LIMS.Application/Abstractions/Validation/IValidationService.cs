namespace LIMS.Application.Abstractions.Validation;

public interface IValidationService
{
    Task ValidateAsync<T>(
        T instance,
        CancellationToken cancellationToken = default);
}
