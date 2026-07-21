namespace RiskEngine;

public readonly struct Territory
{
    public byte OwnerId { get; }
    public ushort TroopCount { get; }

    public Territory(byte ownerId, ushort troopCount)
    {
        OwnerId = ownerId;
        TroopCount = troopCount;
    }

    //Set Troops of Territory
    public Territory WithTroops(ushort newTroopCount)
    {
        return new Territory(OwnerId, newTroopCount);
    }

    //Set Owner and Troops of Territory
    public Territory WithOwner(byte newOwnerId, ushort newTroopCount)
    {
        return new Territory(newOwnerId, newTroopCount);
    }
}