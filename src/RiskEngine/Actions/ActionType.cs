namespace RiskEngine.Actions;

/// <summary>
///     Defines all discrete player action types in the Risk engine.
/// </summary>
public enum ActionType : byte
{
    NotInitialized,
    TurnInCards,
    Reinforce,
    Attack,
    Defend,
    Conquer,
    Fortify,
    SkipPhase,
    EndTurn
}