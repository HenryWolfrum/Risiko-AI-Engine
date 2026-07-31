/// <summary> Zero-allocation view for fortification decision metadata. </summary>
public readonly struct FortifyData
{
    public byte SourceTerritory { get; }
    public byte TargetTerritory { get; }

    public FortifyData(byte source, byte target)
    {
        SourceTerritory = source;
        TargetTerritory = target;
    }
}