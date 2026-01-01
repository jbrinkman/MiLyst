using MiLyst.Domain.Samples;

namespace MiLyst.Application.Samples;

public interface ITenantScopedRecordRepository
{
    Task AddAsync(TenantScopedRecord record, CancellationToken cancellationToken);

    Task<IReadOnlyList<TenantScopedRecord>> ListAsync(CancellationToken cancellationToken);
}
