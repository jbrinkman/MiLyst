using Microsoft.Extensions.Options;

namespace MiLyst.Api.Tenancy;

public sealed class TenantResolutionMiddleware : IMiddleware
{
    private readonly TenantOptions _options;

    public TenantResolutionMiddleware(IOptions<TenantOptions> options)
    {
        _options = options.Value;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var tenantContext = context.RequestServices.GetRequiredService<TenantContext>();

        if (context.Request.Headers.TryGetValue(_options.HeaderName, out var values))
        {
            var raw = values.ToString();
            if (Guid.TryParse(raw, out var tenantId) && tenantId != Guid.Empty)
            {
                tenantContext.SetTenant(tenantId);
            }
        }

        return next(context);
    }
}
