using MiLyst.Domain.Samples;
using Xunit;

namespace MiLyst.Domain.Tests.Tenancy;

public sealed class TenantScopedTests
{
    [Fact]
    public void TenantScopedRecord_AllowsTenantIdToBeSet()
    {
        var tenantId = Guid.NewGuid();
        var record = new TenantScopedRecord { TenantId = tenantId };

        Assert.Equal(tenantId, record.TenantId);
    }
}
