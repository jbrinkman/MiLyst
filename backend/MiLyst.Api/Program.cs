using System.Diagnostics;
using System.Net.Sockets;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var routes = new[]
    {
        new RouteConfig
        {
            RouteId = "vite",
            ClusterId = "vite",
            Match = new RouteMatch { Path = "/{**catch-all}" },
        },
    };

    var clusters = new[]
    {
        new ClusterConfig
        {
            ClusterId = "vite",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                ["vite"] = new() { Address = "http://localhost:5173/" },
            },
        },
    };

    builder.Services.AddReverseProxy().LoadFromMemory(routes, clusters);
}

var app = builder.Build();

var api = app.MapGroup("/api");
api.MapGet("/", () => Results.Text("MiLyst API", "text/plain"));
api.MapGet("/health", () => Results.Ok(new { status = "ok" }));

if (app.Environment.IsDevelopment())
{
    var viteProcess = StartViteDevServer(app.Environment.ContentRootPath, app.Logger);
    if (viteProcess is not null)
    {
        app.Lifetime.ApplicationStopping.Register(() =>
        {
            try
            {
                if (!viteProcess.HasExited)
                {
                    viteProcess.Kill(entireProcessTree: true);
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogDebug(ex, "Failed to stop Vite dev server process.");
            }
        });
    }
    app.MapReverseProxy();
}
else
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapFallbackToFile("index.html");
}

app.Run();

static Process? StartViteDevServer(string backendProjectDir, ILogger logger)
{
    if (IsPortOpen("127.0.0.1", 5173, TimeSpan.FromMilliseconds(150)))
    {
        logger.LogInformation("Vite dev server already running on http://localhost:5173");
        return null;
    }

    var repoRoot = Directory.GetParent(backendProjectDir)?.Parent?.FullName;
    if (repoRoot is null)
    {
        logger.LogWarning("Unable to locate repository root to start Vite.");
        return null;
    }

    var frontendDir = Path.Combine(repoRoot, "frontend");
    if (!Directory.Exists(frontendDir))
    {
        logger.LogWarning("Frontend directory not found at {FrontendDir}", frontendDir);
        return null;
    }

    try
    {
        var psi = new ProcessStartInfo
        {
            FileName = "npm",
            Arguments = "run dev -- --host",
            WorkingDirectory = frontendDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var process = Process.Start(psi);
        if (process is null)
        {
            logger.LogWarning("Failed to start Vite dev server.");
            return null;
        }

        _ = Task.Run(async () =>
        {
            while (true)
            {
                var line = await process.StandardOutput.ReadLineAsync();
                if (line is null)
                {
                    break;
                }
                if (!string.IsNullOrWhiteSpace(line))
                {
                    logger.LogInformation("[vite] {Line}", line);
                }
            }
        });

        _ = Task.Run(async () =>
        {
            while (true)
            {
                var line = await process.StandardError.ReadLineAsync();
                if (line is null)
                {
                    break;
                }
                if (!string.IsNullOrWhiteSpace(line))
                {
                    logger.LogError("[vite] {Line}", line);
                }
            }
        });

        return process;
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Failed to start Vite dev server.");
        return null;
    }
}

static bool IsPortOpen(string host, int port, TimeSpan timeout)
{
    try
    {
        using var client = new TcpClient();
        var task = client.ConnectAsync(host, port);
        return task.Wait(timeout) && client.Connected;
    }
    catch
    {
        return false;
    }
}
