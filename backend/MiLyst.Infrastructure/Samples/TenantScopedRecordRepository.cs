using Microsoft.EntityFrameworkCore;
using MiLyst.Application.Samples;
using MiLyst.Domain.Samples;
using MiLyst.Infrastructure.Persistence;

namespace MiLyst.Infrastructure.Samples;

public sealed class TenantScopedRecordRepository : ITenantScopedRecordRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TenantScopedRecordRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(TenantScopedRecord record, CancellationToken cancellationToken)
    {
        _dbContext.TenantScopedRecords.Add(record);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TenantScopedRecord>> ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.TenantScopedRecords
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
