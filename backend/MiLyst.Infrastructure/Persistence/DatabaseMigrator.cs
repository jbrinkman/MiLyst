using Microsoft.EntityFrameworkCore;
using MiLyst.Application.Persistence;

namespace MiLyst.Infrastructure.Persistence;

public sealed class DatabaseMigrator : IDatabaseMigrator
{
    private readonly ApplicationDbContext _dbContext;

    public DatabaseMigrator(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task MigrateAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Database.MigrateAsync(cancellationToken);
    }
}
