namespace RiskEngine.State.Generation;

/// <summary>
/// Defines the categories of decisions presented to an external decision-maker (AI, UI, Network Client).
/// </summary>
public enum DecisionKind : byte
{
    CardTurnIn,
    Reinforce,
    Attack,
    Defend,
    Conquer,
    Fortify,
    SkipPhase,
    EndTurn,
}