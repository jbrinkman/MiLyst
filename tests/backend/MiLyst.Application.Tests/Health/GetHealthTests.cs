using MiLyst.Application.Health;
using Xunit;

namespace MiLyst.Application.Tests.Health;

public sealed class GetHealthTests
{
    [Fact]
    public void Execute_ReturnsOk()
    {
        var result = GetHealth.Execute();
        Assert.Equal("ok", result.Status);
    }
}
