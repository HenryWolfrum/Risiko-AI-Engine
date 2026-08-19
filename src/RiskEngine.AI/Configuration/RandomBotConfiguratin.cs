namespace RiskEngine.AI.Configuration;

public sealed class RandomBotConfiguration : PlayerConfiguration
{

    public override AgentType Type => AgentType.RandomBot;
    public required ulong Seed { get; init; }
}