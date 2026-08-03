/// <summary> Zero-allocation view for attack decision metadata. </summary>
public readonly struct ConquerData
{
    public byte SourceTerritory { get; }
    public byte TargetTerritory { get; }

    public ConquerData(byte source, byte target)
    {
        SourceTerritory = source;
        TargetTerritory = target;
    }
}