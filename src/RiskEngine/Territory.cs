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

    // Fügt Truppen hinzu und verhindert Überläufe
    public TerritoryState AddTroops(ushort amount)
    {
        int result = TroopCount + amount;

        if (result > ushort.MaxValue)
        {
            return new TerritoryState(OwnerId, ushort.MaxValue);
        }

        return new TerritoryState(OwnerId, (ushort)result);
    }

    // Zieht Truppen ab und lässt mindestens 1 Truppe stehen
    public TerritoryState RemoveTroops(ushort amount)
    {
        if (amount >= TroopCount)
        {
            return new TerritoryState(OwnerId, 1);
        }

        return new TerritoryState(OwnerId, (ushort)(TroopCount - amount));
    }

    // Land wird erobert und bekommt neue Truppen
    public TerritoryState Conquer(byte newOwnerId, ushort occupyingTroops)
    {
        if (occupyingTroops == 0)
        {
            return new TerritoryState(newOwnerId, 1);
        }

        return new TerritoryState(newOwnerId, occupyingTroops);
    }
}