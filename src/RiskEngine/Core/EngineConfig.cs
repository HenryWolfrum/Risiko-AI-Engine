namespace RiskEngine.State;

/// <summary>
/// Immutable configuration used to create a game.
/// </summary>
public readonly struct EngineConfig
{
    public byte PlayerCount { get; }
    
    public ushort MaxRounds { get; }


    public EngineConfig(byte playerCount = EngineConstants.DEFAULT_PLAYERS, ushort maxRounds = EngineConstants.MAX_ROUNDS)
    {
        PlayerCount = playerCount;
        MaxRounds = maxRounds;
    }
}