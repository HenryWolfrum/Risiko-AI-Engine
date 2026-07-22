namespace RiskEngine;

public readonly struct EngineConfig
{
    public byte PlayerCount { get; }

    public byte TerritoryCount { get; }

    public ushort MaxRounds { get; }


    public EngineConfig(byte playerCount, byte territoryCount=EngineConstants.DEFAULT_TERRITORY_COUNT, ushort maxRounds = EngineConstants.MAX_ROUNDS)
    {
        PlayerCount = playerCount;
        TerritoryCount = territoryCount;
        MaxRounds = maxRounds;
    }
}