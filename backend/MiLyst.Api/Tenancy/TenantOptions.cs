namespace MiLyst.Api.Tenancy;

public sealed class TenantOptions
{
    public const string SectionName = "Tenancy";

    public string HeaderName { get; set; } = "X-Tenant-Id";
}
