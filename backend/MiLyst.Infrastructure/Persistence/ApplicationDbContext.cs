using Microsoft.EntityFrameworkCore;
using MiLyst.Application.Tenancy;
using MiLyst.Domain.Samples;
using MiLyst.Domain.Tenancy;

namespace MiLyst.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public Guid TenantId => _tenantContext.TenantId;

    public bool HasTenant => _tenantContext.HasTenant;

    public DbSet<TenantScopedRecord> TenantScopedRecords => Set<TenantScopedRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TenantScopedRecord>(b =>
        {
            b.ToTable("TenantScopedRecords");
            b.HasKey(x => x.Id);

            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.Value).HasMaxLength(500);
            b.Property(x => x.CreatedAt).IsRequired();

            b.HasIndex(x => new { x.TenantId, x.CreatedAt });

            b.HasQueryFilter(x => x.TenantId == TenantId);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTenantIds();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyTenantIds();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyTenantIds();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyTenantIds();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyTenantIds()
    {
        var tenantScopedEntries = ChangeTracker.Entries<ITenantScoped>().ToList();

        if (!HasTenant)
        {
            if (tenantScopedEntries.Any(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
            {
                throw new InvalidOperationException("Tenant-scoped changes cannot be saved without a tenant context.");
            }

            return;
        }

        foreach (var entry in tenantScopedEntries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(ITenantScoped.TenantId)).CurrentValue = TenantId;
            }

            if (entry.State == EntityState.Modified)
            {
                var tenantIdProperty = entry.Property(x => x.TenantId);
                if (tenantIdProperty.IsModified)
                {
                    throw new InvalidOperationException("TenantId cannot be modified once set.");
                }
            }
        }
    }
}
