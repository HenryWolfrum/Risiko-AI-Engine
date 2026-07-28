namespace RiskEngine;

/// <summary>
/// Immutable configuration used to create a game.
/// </summary>
public readonly struct EngineConfig
{
    public byte PlayerCount { get; }

    public byte TerritoryCount { get; }

    public ushort MaxRounds { get; }


    public EngineConfig(byte playerCount = EngineConstants.DEFAULT_PLAYERS, byte territoryCount = EngineConstants.MAX_TERRITORIES, ushort maxRounds = EngineConstants.DEFAULT_MAX_ROUNDS)
    {
        if (playerCount < EngineConstants.MIN_PLAYERS || playerCount > EngineConstants.MAX_PLAYERS)
        {
            throw new ArgumentOutOfRangeException(nameof(playerCount));
        }

        if (territoryCount == 0 || territoryCount > EngineConstants.MAX_TERRITORIES)
        {
            throw new ArgumentOutOfRangeException(nameof(territoryCount));
        }

        if (maxRounds == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRounds));
        }

        PlayerCount = playerCount;
        TerritoryCount = territoryCount;
        MaxRounds = maxRounds;
    }
}