using Microsoft.EntityFrameworkCore;
using MiLyst.Application.Tenancy;
using MiLyst.Domain.Samples;
using MiLyst.Domain.Tenancy;

namespace MiLyst.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    private readonly Guid _tenantId;
    private readonly bool _hasTenant;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantId = tenantContext.TenantId;
        _hasTenant = tenantContext.HasTenant;
    }

    public Guid TenantId => _tenantId;

    public bool HasTenant => _hasTenant;

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

    private void ApplyTenantIds()
    {
        var tenantScopedEntries = ChangeTracker.Entries<ITenantScoped>().ToList();

        if (!_hasTenant)
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
                entry.Property(nameof(ITenantScoped.TenantId)).CurrentValue = _tenantId;
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
