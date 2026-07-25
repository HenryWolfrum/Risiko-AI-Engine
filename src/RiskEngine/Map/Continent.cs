namespace RiskEngine;


public readonly struct Continent
{
    public byte Id { get; }
    public string Name { get; }
    public byte BonusTroops { get; }
    public byte TerritoryCount { get; }

    public Continent(byte id, string name, byte bonusTroops, byte territoryCount)
    {
        Id = id;
        Name = name;
        BonusTroops = bonusTroops;
        TerritoryCount = territoryCount;
    }
}