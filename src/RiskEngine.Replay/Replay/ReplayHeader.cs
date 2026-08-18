using RiskEngine.AI.Configuration;
using RiskEngine.State;

namespace RiskEngine.Replay;

/// <summary>
/// Stores metadata required to identify and describe a replay.
/// </summary>
public readonly struct ReplayHeader
{
    /// <summary>
    /// Random seed used to initialize the simulation.
    /// </summary>
    public ulong Seed { get; init; }

    /// <summary>
    /// Game layout used for the simulation.
    /// </summary>
    public required GameLayout Layout { get; init; }

    /// <summary>
    /// Configuration of players participating in the simulation.
    /// </summary>
    public required PlayerConfiguration[] PlayerConfigs { get; init; }
}