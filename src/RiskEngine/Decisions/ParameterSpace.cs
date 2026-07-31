

using RiskEngine.State;

/// <summary>
/// Represents a compact, continuous integer interval for parameterized choices (e.g., troop counts, dice counts).
/// </summary>
public readonly struct ParameterSpace
{
    public static ParameterSpace None => new(EngineConstants.NO_VALUE, EngineConstants.NO_VALUE);

    public byte Min { get; }
    public byte Max { get; }

    public bool HasChoice => Max > Min;
    public bool IsEmpty => Min == EngineConstants.NO_VALUE && Max == EngineConstants.NO_VALUE;

    public ParameterSpace(byte min, byte max)
    {
        Min = min;
        Max = max;
    }

    public bool Contains(byte value) => value >= Min && value <= Max;
}