namespace RiskEngine;

public readonly struct TerritoryState
{
    public byte OwnerId { get; }
    public ushort TroopCount { get; }

    public TerritoryState(byte ownerId, ushort troopCount)
    {
        OwnerId = ownerId;
        TroopCount = troopCount;
    }

    // Erstellt einen neuen Zustand mit veränderter Truppenzahl
    public TerritoryState WithTroops(ushort newTroopCount)
    {
        return new TerritoryState(OwnerId, newTroopCount);
    }

    // Erstellt einen neuen Zustand mit neuem Besitzer und neuer Truppenzahl
    public TerritoryState WithOwner(byte newOwnerId, ushort newTroopCount)
    {
        return new TerritoryState(newOwnerId, newTroopCount);
    }
}