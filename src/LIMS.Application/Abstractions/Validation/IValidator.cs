using System.ComponentModel.DataAnnotations;

namespace LIMS.Application.Abstractions.Validation;

public interface IValidator<in T>
{
    Task<ValidationResult> ValidateAsync(
        T instance,
        CancellationToken cancellationToken = default);
}
