/// <summary> Zero-allocation view for reinforcement decision metadata. </summary>
public readonly struct ReinforceData
{
    public byte TargetTerritory { get; }

    public ReinforceData(byte target)
    {
        TargetTerritory = target;
    }
}