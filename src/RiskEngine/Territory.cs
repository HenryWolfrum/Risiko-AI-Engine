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

    //Set Troops of Territory
    public TerritoryState WithTroops(ushort newTroopCount)
    {
        return new TerritoryState(OwnerId, newTroopCount);
    }

    //Set Owner and Troops of Territory
    public TerritoryState WithOwner(byte newOwnerId, ushort newTroopCount)
    {
        return new TerritoryState(newOwnerId, newTroopCount);
    }
}