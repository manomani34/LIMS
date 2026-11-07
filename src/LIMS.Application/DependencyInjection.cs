using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LIMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped<IValidationService, ValidationService>();

        RegisterHandlersAndValidators(services);

        return services;
    }

    private static void RegisterHandlersAndValidators(
        IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        var implementationTypes = assembly
            .GetTypes()
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                !type.IsGenericTypeDefinition);

        foreach (var implementationType in implementationTypes)
        {
            var serviceTypes = implementationType
                .GetInterfaces()
                .Where(IsApplicationServiceInterface);

            foreach (var serviceType in serviceTypes)
            {
                services.TryAddEnumerable(
                    ServiceDescriptor.Scoped(
                        serviceType,
                        implementationType));
            }
        }
    }

    private static bool IsApplicationServiceInterface(
        Type serviceType)
    {
        if (!serviceType.IsGenericType)
            return false;

        var genericType = serviceType.GetGenericTypeDefinition();

        return genericType == typeof(ICommandHandler<,>)
               || genericType == typeof(IQueryHandler<,>)
               || genericType == typeof(IValidator<>);
    }
}
