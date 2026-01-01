using System.Net;
using System.Net.Http.Json;
using MiLyst.Api.IntegrationTests.Infrastructure;
using Xunit;

namespace MiLyst.Api.IntegrationTests.Tenancy;

public sealed class TenantIsolationTests : IClassFixture<PostgresWebAppFactory>
{
    private const string TenantHeader = "X-Tenant-Id";

    private readonly PostgresWebAppFactory _factory;

    public TenantIsolationTests(PostgresWebAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Records_AreIsolatedByTenant()
    {
        Assert.True(_factory.IsAvailable, _factory.UnavailableReason);

        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        var client = _factory.CreateClient();

        await CreateRecordAsync(client, tenantA, "a1");
        await CreateRecordAsync(client, tenantB, "b1");

        var a = await ListAsync(client, tenantA);
        var b = await ListAsync(client, tenantB);

        Assert.Single(a);
        Assert.Single(b);

        Assert.Equal("a1", a[0].Value);
        Assert.Equal("b1", b[0].Value);
    }

    private static async Task CreateRecordAsync(HttpClient client, Guid tenantId, string value)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/sample/records")
        {
            Content = JsonContent.Create(new { value }),
        };

        request.Headers.Add(TenantHeader, tenantId.ToString());

        var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static async Task<List<RecordDto>> ListAsync(HttpClient client, Guid tenantId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/sample/records");
        request.Headers.Add(TenantHeader, tenantId.ToString());

        var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var records = await response.Content.ReadFromJsonAsync<List<RecordDto>>();
        Assert.NotNull(records);
        return records;
    }

    public sealed record RecordDto(Guid Id, Guid TenantId, string? Value, DateTimeOffset CreatedAt);
}
