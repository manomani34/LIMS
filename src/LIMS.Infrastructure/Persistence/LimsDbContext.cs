
using LIMS.Domain.Approvals;
using LIMS.Domain.Experiments;
using LIMS.Domain.Files.Entities;
using LIMS.Domain.Instruments;
using LIMS.Domain.Inventory;
using LIMS.Domain.Laboratories;
using LIMS.Domain.Protocols;
using LIMS.Domain.Requests;
using LIMS.Domain.Results;
using LIMS.Domain.Samples;
using LIMS.Domain.Workflows;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Infrastructure.Persistence;

public sealed class LimsDbContext : DbContext
{
    public LimsDbContext(DbContextOptions<LimsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Experiment> Experiments => Set<Experiment>();

    public DbSet<FileAsset> FileAssets => Set<FileAsset>();

    public DbSet<FileReference> FileReferences => Set<FileReference>();

    public DbSet<Instrument> Instruments => Set<Instrument>();

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    public DbSet<Laboratory> Laboratories => Set<Laboratory>();

    public DbSet<Protocol> Protocols => Set<Protocol>();

    public DbSet<SampleRequest> SampleRequests => Set<SampleRequest>();

    public DbSet<AnalysisResult> AnalysisResults => Set<AnalysisResult>();

    public DbSet<Sample> Samples => Set<Sample>();

    public DbSet<Approval> Approvals => Set<Approval>();

    public DbSet<WorkflowDefinition> WorkflowDefinitions =>
        Set<WorkflowDefinition>();

    public DbSet<WorkflowInstance> WorkflowInstances =>
        Set<WorkflowInstance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LimsDbContext).Assembly);

        IgnoreDomainEvents(modelBuilder);
    }

    private static void IgnoreDomainEvents(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Experiment>().Ignore(x => x.DomainEvents);
        modelBuilder.Entity<FileAsset>().Ignore(x => x.DomainEvents);
        modelBuilder.Entity<FileReference>().Ignore(x => x.DomainEvents);
        modelBuilder.Entity<Instrument>().Ignore(x => x.DomainEvents);
        modelBuilder.Entity<InventoryItem>().Ignore(x => x.DomainEvents);
        modelBuilder.Entity<Laboratory>().Ignore(x => x.DomainEvents);
        modelBuilder.Entity<Protocol>().Ignore(x => x.DomainEvents);
        modelBuilder.Entity<SampleRequest>().Ignore(x => x.DomainEvents);
        modelBuilder.Entity<AnalysisResult>().Ignore(x => x.DomainEvents);
        modelBuilder.Entity<Sample>().Ignore(x => x.DomainEvents);
        modelBuilder.Entity<Approval>().Ignore(x => x.DomainEvents);
        modelBuilder.Entity<WorkflowDefinition>().Ignore(x => x.DomainEvents);
        modelBuilder.Entity<WorkflowInstance>().Ignore(x => x.DomainEvents);
    }
}
