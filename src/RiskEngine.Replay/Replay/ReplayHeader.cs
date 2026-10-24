using RiskEngine.Setup.Configuration;

namespace RiskEngine.Replay;

/// <summary>
///     Stores metadata required to identify and describe a replay.
/// </summary>
public readonly struct ReplayHeader
{
    /// <summary>
    ///     Complete description of the simulated game (seed, layout, players).
    /// </summary>
    public required GameConfiguration Configuration { get; init; }
}