
using LIMS.Application.Abstractions.Identity;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Security;
using LIMS.Application.Abstractions.Time;

using LIMS.Application.Experiments.Abstractions;
using LIMS.Application.Files.Repositories;
using LIMS.Application.Instruments.Repositories;
using LIMS.Application.Inventory.Repositories;
using LIMS.Application.Laboratories.Interfaces;
using LIMS.Application.Protocols.Abstractions;
using LIMS.Application.Requests.Repositories;
using LIMS.Application.Results.Repositories;
using LIMS.Application.Samples.Interfaces;

using LIMS.Infrastructure.Identity.Authorization;
using LIMS.Infrastructure.Identity.Users;
using LIMS.Infrastructure.Messaging;
using LIMS.Infrastructure.Persistence;
using LIMS.Infrastructure.Persistence.Repositories;
using LIMS.Infrastructure.Time;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using LIMS.Application.Abstractions.Storage;
using LIMS.Infrastructure.Storage;

namespace LIMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IPermissionChecker, PermissionChecker>();

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        var connectionString =
            configuration.GetConnectionString("LimsDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'LimsDatabase' is not configured.");
        }

        services.AddDbContext<LimsDbContext>(options =>
            options.UseSqlServer(connectionString));

        RegisterRepositories(services);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEventPublisher, DomainEventPublisher>();

        services.AddSingleton<IObjectStorage, LocalObjectStorage>();

        return services;
    }

    private static void RegisterRepositories(
        IServiceCollection services)
    {
        services.AddScoped<IExperimentRepository, ExperimentRepository>();
        services.AddScoped<IInstrumentRepository, InstrumentRepository>();
        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
        services.AddScoped<ILaboratoryRepository, LaboratoryRepository>();
        services.AddScoped<IProtocolRepository, ProtocolRepository>();
        services.AddScoped<ISampleRequestRepository, SampleRequestRepository>();
        services.AddScoped<IAnalysisResultRepository, AnalysisResultRepository>();
        services.AddScoped<ISampleRepository, SampleRepository>();

        services.AddScoped<IFileAssetRepository, FileAssetRepository>();
        services.AddScoped<IFileReferenceRepository, FileReferenceRepository>();
    }
}
