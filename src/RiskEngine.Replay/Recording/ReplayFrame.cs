using RiskEngine.State;

namespace RiskEngine.Replay;

/// <summary>
/// Represents a single recorded state transition during a simulation.
/// </summary>
public struct ReplayFrame
{
    /// <summary>
    /// Game state after the action has been applied.
    /// </summary>
    public GameState State;

    /// <summary>
    /// Action that produced this state.
    /// </summary>
    public GameAction? Action;
}