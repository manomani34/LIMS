using LIMS.Application.Abstractions.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.Application.Behaviors;

public sealed class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ValidateAsync<T>(
        T instance,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(instance);

        var validators = _serviceProvider
            .GetServices<IValidator<T>>()
            .ToArray();

        if (validators.Length == 0)
            return;

        var failures = new List<ValidationFailure>();

        foreach (var validator in validators)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await validator.ValidateAsync(
                instance,
                cancellationToken);

            if (result.IsInvalid)
                failures.AddRange(result.Errors);
        }

        if (failures.Count > 0)
            throw new ValidationException(failures);
    }
}
