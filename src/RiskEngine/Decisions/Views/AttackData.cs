
/// <summary> Zero-allocation view for attack decision metadata. </summary>
public readonly struct AttackData
{
    public byte SourceTerritory { get; }
    public byte TargetTerritory { get; }

    public AttackData(byte source, byte target)
    {
        SourceTerritory = source;
        TargetTerritory = target;
    }
}