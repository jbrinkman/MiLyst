using MiLyst.Domain.Samples;
using Xunit;

namespace MiLyst.Domain.Tests.Tenancy;

public sealed class TenantScopedTests
{
    [Fact]
    public void TenantScopedRecord_HasNonPublicTenantIdSetter()
    {
        var tenantIdProperty = typeof(TenantScopedRecord).GetProperty(nameof(TenantScopedRecord.TenantId));
        Assert.NotNull(tenantIdProperty);

        Assert.False(tenantIdProperty!.SetMethod?.IsPublic ?? false);
    }
}
