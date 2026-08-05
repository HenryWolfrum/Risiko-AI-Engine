namespace RiskEngine.AI.Configuration;

public sealed class RandomBotConfiguration : PlayerConfiguration
{
    public required int Seed { get; init; }
}