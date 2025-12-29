using MiLyst.Application.Tenancy;

namespace MiLyst.Api.Tenancy;

public sealed class TenantContext : ITenantContext
{
    public Guid TenantId { get; private set; }

    public bool HasTenant { get; private set; }

    internal void Clear()
    {
        TenantId = default;
        HasTenant = false;
    }

    internal void SetTenant(Guid tenantId)
    {
        TenantId = tenantId;
        HasTenant = true;
    }
}
