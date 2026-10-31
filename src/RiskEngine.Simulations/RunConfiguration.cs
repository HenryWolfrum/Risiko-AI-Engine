namespace RiskEngine.Simulations;

public sealed record RunConfiguration
{
    public bool UseParallelism { get; init; } = false;
}