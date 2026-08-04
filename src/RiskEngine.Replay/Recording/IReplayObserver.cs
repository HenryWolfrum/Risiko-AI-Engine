using RiskEngine.State;

namespace RiskEngine.Replay.Recording;

/// <summary>
/// Observes state changes during a simulation.
/// </summary>
public interface IReplayObserver
{
    /// <summary>
    /// Records a new simulation frame.
    /// </summary>
    /// <param name="state">Current game state.</param>
    /// <param name="action">Action that produced the state.</param>
    void Record(
        in GameState state,
        in GameAction action);
}