using MiLyst.Api.Tenancy;
using Xunit;

namespace MiLyst.Api.IntegrationTests.Tenancy;

public sealed class TenantContextTests
{
    [Fact]
    public void SetTenant_WithGuidEmpty_Throws()
    {
        var context = new TenantContext();

        var ex = Assert.Throws<ArgumentException>(() => context.SetTenant(Guid.Empty));
        Assert.Equal("tenantId", ex.ParamName);
    }

    [Fact]
    public void SetTenant_SetsTenantIdAndHasTenant()
    {
        var context = new TenantContext();
        var tenantId = Guid.NewGuid();

        context.SetTenant(tenantId);

        Assert.True(context.HasTenant);
        Assert.Equal(tenantId, context.TenantId);
    }

    [Fact]
    public void Clear_ResetsTenantIdAndHasTenant()
    {
        var context = new TenantContext();
        context.SetTenant(Guid.NewGuid());

        context.Clear();

        Assert.False(context.HasTenant);
        Assert.Equal(Guid.Empty, context.TenantId);
    }
}
