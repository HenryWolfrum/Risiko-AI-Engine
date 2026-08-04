using RiskEngine.State;

namespace RiskEngine.Replay;

/// <summary>
/// Stores metadata required to identify and describe a replay.
/// </summary>
public struct ReplayHeader
{
    /// <summary>
    /// Random seed used to initialize the simulation.
    /// </summary>
    public int Seed;

    /// <summary>
    /// Game layout used for the simulation.
    /// </summary>
    public required GameLayout Layout;

    /// <summary>
    /// Optional engine version.
    /// </summary>
    public string? EngineVersion;

    /// <summary>
    /// Time when the replay was recorded.
    /// </summary>
    public DateTime Timestamp;
}