using MiLyst.Application.Tenancy;

namespace MiLyst.Api.Tenancy;

public static class DependencyInjection
{
    public static IServiceCollection AddTenancy(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TenantOptions>(configuration.GetSection(TenantOptions.SectionName));

        services.AddScoped<TenantContext>();
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());

        services.AddScoped<TenantResolutionMiddleware>();

        return services;
    }
}
