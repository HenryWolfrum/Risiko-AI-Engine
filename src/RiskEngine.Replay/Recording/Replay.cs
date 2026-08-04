namespace RiskEngine.Replay;

/// <summary>
/// Represents a complete replay of a simulated game.
/// Contains metadata and the recorded simulation frames.
/// </summary>
public sealed class Replay
{
    /// <summary>
    /// General metadata describing the replay.
    /// </summary>
    public ReplayHeader Header { get; init; }

    /// <summary>
    /// Chronological list of recorded simulation frames.
    /// </summary>
    public List<ReplayFrame> Frames { get; } = [];
}