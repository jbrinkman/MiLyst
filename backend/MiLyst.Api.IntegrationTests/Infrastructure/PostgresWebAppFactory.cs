using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Testcontainers.PostgreSql;
using Xunit;

namespace MiLyst.Api.IntegrationTests.Infrastructure;

public sealed class PostgresWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string Database = "milyst_test";
    private const string Username = "milyst";
    private const string Password = "milyst";

    private int? _mappedPort;

    private PostgreSqlContainer? _postgres;

    public bool IsAvailable { get; private set; } = true;

    public string? UnavailableReason { get; private set; }

    async Task IAsyncLifetime.InitializeAsync()
    {
        try
        {
            if (!TryConfigureDockerHost(out var dockerHostReason))
            {
                IsAvailable = false;
                UnavailableReason = dockerHostReason;
                return;
            }

            _postgres = new PostgreSqlBuilder()
                .WithImage("postgres:16-alpine")
                .WithDatabase(Database)
                .WithUsername(Username)
                .WithPassword(Password)
                .WithPortBinding(5432, true)
                .Build();

            await _postgres.StartAsync();

            _mappedPort = _postgres.GetMappedPublicPort(5432);
            await WaitForPortAsync(_postgres.Hostname, _mappedPort.Value, TimeSpan.FromSeconds(20));
        }
        catch (Exception ex)
        {
            IsAvailable = false;
            UnavailableReason = $"Postgres container could not start (is Docker running?). {ex}";
        }
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (!IsAvailable)
        {
            return;
        }

        if (_postgres is null)
        {
            return;
        }

        await _postgres.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            if (!IsAvailable)
            {
                return;
            }

            if (_postgres is null)
            {
                return;
            }

            var connectionString = _postgres.GetConnectionString();

            var overrides = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString,
                ["Tenancy:HeaderName"] = "X-Tenant-Id",
            };

            config.AddInMemoryCollection(overrides);
        });
    }

    private static bool TryConfigureDockerHost(out string? unavailableReason)
    {
        unavailableReason = null;

        var dockerHost = Environment.GetEnvironmentVariable("DOCKER_HOST");
        if (!string.IsNullOrWhiteSpace(dockerHost))
        {
            return true;
        }

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TESTCONTAINERS_RYUK_DISABLED")))
        {
            Environment.SetEnvironmentVariable("TESTCONTAINERS_RYUK_DISABLED", "true");
        }

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        var candidates = new[]
        {
            Path.Combine(home, ".docker", "run", "docker.sock"),
            "/var/run/docker.sock",
            Path.Combine(home, "Library", "Containers", "com.docker.docker", "Data", "docker-cli.sock"),
        };

        var socketPath = candidates.FirstOrDefault(File.Exists);
        if (socketPath is null)
        {
            unavailableReason = "Docker socket not found for integration tests. Ensure Docker Desktop is running or set DOCKER_HOST to a valid unix socket.";
            return false;
        }

        Environment.SetEnvironmentVariable("DOCKER_HOST", $"unix://{socketPath}");
        return true;
    }

    private static async Task WaitForPortAsync(string host, int port, TimeSpan timeout)
    {
        var start = DateTimeOffset.UtcNow;

        while (DateTimeOffset.UtcNow - start < timeout)
        {
            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(host, port);
                var completed = await Task.WhenAny(connectTask, Task.Delay(250));
                if (completed == connectTask && client.Connected)
                {
                    return;
                }
            }
            catch
            {
                // ignore and retry
            }

            await Task.Delay(250);
        }

        throw new TimeoutException($"Timed out waiting for {host}:{port} to accept connections.");
    }
}
