namespace RiskEngine.Mission;

using System;

public readonly struct MissionCatalog
{
    private readonly MissionDefinition[] _missions;
    
    public readonly byte FallbackMissionId;

    public MissionCatalog(MissionDefinition[] missions, byte fallbackMissionId)
    {
        _missions = missions ?? Array.Empty<MissionDefinition>();
        
        FallbackMissionId = fallbackMissionId;
    }

    /// <summary>
    /// Gets the total number of registered missions in this catalog.
    /// </summary>
    public int Count => _missions.Length;

    /// <summary>
    /// Returns a direct read-only reference to the mission definition by index (0-allocation).
    /// </summary>
    public ref readonly MissionDefinition this[int index] => ref _missions[index];

    /// <summary>
    /// Returns the underlying missions as a ReadOnlySpan for fast iteration.
    /// </summary>
    public ReadOnlySpan<MissionDefinition> AsSpan() => _missions.AsSpan();
}