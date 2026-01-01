using System.Net;
using System.Net.Http.Json;
using MiLyst.Api.IntegrationTests.Infrastructure;
using Xunit;

namespace MiLyst.Api.IntegrationTests.Health;

public sealed class HealthEndpointTests : IClassFixture<PostgresWebAppFactory>
{
    private readonly PostgresWebAppFactory _factory;

    public HealthEndpointTests(PostgresWebAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        Assert.True(_factory.IsAvailable, _factory.UnavailableReason);

        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(body);
        Assert.Equal("ok", body["status"]);
    }
}
