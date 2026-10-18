using RiskEngine.Actions;
using RiskEngine.State;

namespace RiskEngine.Observer;

/// <summary>
/// Observes lifecycle events and actions during a game.
/// </summary>
public interface IGameObserver
{
    /// <summary>
    /// Called when the game has been initialized.
    /// </summary>
    void OnGameStarted(in GameState state);

    /// <summary>
    /// Called after a validated action has been successfully applied.
    /// </summary>
    void OnActionApplied(in GameState state, in GameAction action);

    /// <summary>
    /// Called when the game has finished.
    /// </summary>
    void OnGameFinished(in GameState state);
}