using RiskEngine.State;

namespace RiskEngine.Observer;

/// <summary>
/// Observes state changes during a simulation.
/// </summary>
public interface IGameObserver
{
    /// <summary>
    /// Records a new simulation frame.
    /// </summary>
    /// <param name="state">Current game state.</param>
    /// <param name="action">Action that produced the state.</param>
    void Record(in GameState state, in GameAction action);
    
    void RecordInitialState(in GameState state);
    
    /// <summary>
    /// Records the final state after the game has ended.
    /// </summary>
    void RecordFinalState(in GameState state);
}