namespace RiskEngine.AI.Configuration;

/// <summary>
/// Base type for all player configurations.
/// A configuration describes how an <see cref="IRiskPlayer"/> should be created,
/// independent of its runtime state.
/// </summary>
public abstract class PlayerConfiguration
{
    public abstract AgentType Type { get;}
 
}