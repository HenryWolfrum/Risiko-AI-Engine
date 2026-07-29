namespace RiskEngine.State;

public interface IRiskPlayer
{
    /// <summary>
    ///     Evaluates the state and returns the next action for the given phase.
    /// </summary>
    GameAction DecideAction(in GameState state, GamePhase phase, GameLayout game);
    
}