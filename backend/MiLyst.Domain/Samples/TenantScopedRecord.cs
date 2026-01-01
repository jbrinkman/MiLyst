using MiLyst.Domain.Tenancy;

namespace MiLyst.Domain.Samples;

public sealed class TenantScopedRecord : ITenantScoped
{
    public Guid Id { get; private set; }

    public Guid TenantId { get; private set; }

    public string? Value { get; set; }

    public DateTimeOffset CreatedAt { get; init; }

    public static TenantScopedRecord Create(string? value)
    {
        return new TenantScopedRecord
        {
            Id = Guid.NewGuid(),
            Value = value,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
}
