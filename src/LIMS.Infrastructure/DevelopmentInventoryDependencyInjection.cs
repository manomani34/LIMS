using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Inventory.Repositories;
using LIMS.Application.Laboratories.Interfaces;
using LIMS.Infrastructure.Persistence.InMemory;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.Infrastructure;

public static class DevelopmentInventoryDependencyInjection
{
    public static IServiceCollection AddDevelopmentInventoryInMemory(
        this IServiceCollection services)
    {
        services.AddSingleton<InMemoryLimsStore>();

        services.AddScoped<
            IInventoryItemRepository,
            InMemoryInventoryItemRepository>();

        services.AddScoped<
            ILaboratoryRepository,
            InMemoryLaboratoryRepository>();

        services.AddScoped<
            IUnitOfWork,
            InMemoryUnitOfWork>();

        return services;
    }
}
