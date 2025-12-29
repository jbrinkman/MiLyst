namespace MiLyst.Application.Health;

public static class GetHealth
{
    public static Result Execute() => new("ok");

    public sealed record Result(string Status);
}
