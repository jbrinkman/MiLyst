using Microsoft.EntityFrameworkCore;
using MiLyst.Application.Tenancy;
using MiLyst.Domain.Samples;
using MiLyst.Infrastructure.Persistence;
using Xunit;

namespace MiLyst.Infrastructure.Tests.Persistence;

public sealed class ApplicationDbContextTenantEnforcementTests
{
    [Fact]
    public void SaveChanges_WhenNoTenantAndTenantScopedAdded_Throws()
    {
        var tenantContext = new FakeTenantContext(Guid.Empty, hasTenant: false);

        using var db = CreateDbContext(tenantContext);

        db.TenantScopedRecords.Add(
            new TenantScopedRecord
            {
                Id = Guid.NewGuid(),
                Value = "v",
                CreatedAt = DateTimeOffset.UtcNow,
            }
        );

        var ex = Assert.Throws<InvalidOperationException>(() => db.SaveChanges());
        Assert.Equal("Tenant-scoped changes cannot be saved without a tenant context.", ex.Message);
    }

    [Fact]
    public void SaveChanges_WhenTenantPresent_AppliesTenantIdOnAddedEntities()
    {
        var tenantId = Guid.NewGuid();
        var tenantContext = new FakeTenantContext(tenantId, hasTenant: true);

        using var db = CreateDbContext(tenantContext);

        var record = new TenantScopedRecord
        {
            Id = Guid.NewGuid(),
            Value = "v",
            CreatedAt = DateTimeOffset.UtcNow,
        };

        db.TenantScopedRecords.Add(record);
        db.SaveChanges();

        Assert.Equal(tenantId, record.TenantId);
    }

    [Fact]
    public void SaveChanges_WhenTenantIdIsModified_Throws()
    {
        var tenantId = Guid.NewGuid();
        var tenantContext = new FakeTenantContext(tenantId, hasTenant: true);

        using var db = CreateDbContext(tenantContext);

        var record = new TenantScopedRecord
        {
            Id = Guid.NewGuid(),
            Value = "v",
            CreatedAt = DateTimeOffset.UtcNow,
        };

        db.TenantScopedRecords.Add(record);
        db.SaveChanges();

        var entry = db.Entry(record);
        entry.Property(x => x.TenantId).CurrentValue = Guid.NewGuid();

        var ex = Assert.Throws<InvalidOperationException>(() => db.SaveChanges());
        Assert.Equal("TenantId cannot be modified once set.", ex.Message);
    }

    private static ApplicationDbContext CreateDbContext(ITenantContext tenantContext)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options, tenantContext);
    }

    private sealed class FakeTenantContext : ITenantContext
    {
        public FakeTenantContext(Guid tenantId, bool hasTenant)
        {
            TenantId = tenantId;
            HasTenant = hasTenant;
        }

        public Guid TenantId { get; }

        public bool HasTenant { get; }
    }
}
