using MiLyst.Domain.Tenancy;

namespace MiLyst.Domain.Samples;

public sealed class TenantScopedRecord : ITenantScoped
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string? Value { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
