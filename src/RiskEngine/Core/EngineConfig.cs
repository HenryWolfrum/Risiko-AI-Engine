namespace RiskEngine;

/// <summary>
/// Immutable configuration used to create a game.
/// </summary>
public readonly struct EngineConfig
{
    public byte PlayerCount { get; }
    
    public ushort MaxRounds { get; }


    public EngineConfig(byte playerCount = EngineConstants.DEFAULT_PLAYERS, ushort maxRounds = EngineConstants.MAX_ROUNDS)
    {
        if (playerCount < EngineConstants.MIN_PLAYERS || playerCount > EngineConstants.MAX_PLAYERS)
        {
            throw new ArgumentOutOfRangeException(nameof(playerCount));
        }

        if (maxRounds == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRounds));
        }

        PlayerCount = playerCount;
        MaxRounds = maxRounds;
    }
}