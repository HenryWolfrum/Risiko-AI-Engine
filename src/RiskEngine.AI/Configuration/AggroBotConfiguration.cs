namespace RiskEngine.AI.Configuration;

/// <summary>
/// Configuration for a deterministic AggroBot.
/// Holds no parameters as the bot operates strictly on fixed priorities.
/// </summary>
public sealed class AggroBotConfiguration : PlayerConfiguration
{
    public override AgentType Type => AgentType.Aggro;
}