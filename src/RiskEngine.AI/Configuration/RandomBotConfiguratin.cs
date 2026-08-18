namespace RiskEngine.AI.Configuration;

public sealed class RandomBotConfiguration : PlayerConfiguration
{
    public required ulong Seed { get; init; }
}