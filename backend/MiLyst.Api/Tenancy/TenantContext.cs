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
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("tenantId cannot be Guid.Empty", nameof(tenantId));
        }

        TenantId = tenantId;
        HasTenant = true;
    }
}
