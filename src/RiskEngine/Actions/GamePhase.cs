namespace RiskEngine.Actions;

/// <summary>
///     Is used to keep track of the current game phase in order to process the next simulation steps
/// </summary>
public enum GamePhase : byte
{
    Start,
    CardTurnIn,
    Reinforce,
    Attack,
    Defend,
    Conquer,
    Fortify,
    Terminated
}