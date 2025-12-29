namespace MiLyst.Domain.Tenancy;

public interface ITenantScoped
{
    Guid TenantId { get; set; }
}
