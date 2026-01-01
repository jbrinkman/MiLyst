using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiLyst.Application.Persistence;
using MiLyst.Application.Samples;
using MiLyst.Infrastructure.Persistence;
using MiLyst.Infrastructure.Samples;

namespace MiLyst.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = ConnectionStringHelper.GetDefaultConnectionString(configuration);
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IDatabaseMigrator, DatabaseMigrator>();
        services.AddScoped<ITenantScopedRecordRepository, TenantScopedRecordRepository>();

        return services;
    }
}

internal static class ConnectionStringHelper
{
    internal static string GetDefaultConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Missing connection string 'DefaultConnection'. Configure ConnectionStrings:DefaultConnection (or ConnectionStrings__DefaultConnection)."
            );
        }

        return connectionString;
    }
}
