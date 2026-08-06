namespace RiskEngine.State;

/// <summary>
/// Immutable configuration used to create a game.
/// </summary>
public readonly struct EngineConfig
{
    public byte PlayerCount { get; }
    
    public ushort MaxRounds { get; }

    public static readonly EngineConfig Default =
        new(EngineConstants.DEFAULT_PLAYERS, EngineConstants.MAX_ROUNDS);

    public EngineConfig(byte playerCount, ushort maxRounds)
    {
        PlayerCount = playerCount;
        MaxRounds = maxRounds;
    }
}