namespace RiskEngine;

/// <summary>
///     Defines all discrete player action types in the Risk engine.
///     Fixed to byte size to fit zero-allocation structs efficiently.
/// </summary>
public enum ActionType : byte
{
    TurnInCards, // Trade in 3 matching/distinct cards for troops
    Reinforce, // Place troops during reinforcement phase
    Attack, // Execute an attack from source to target territory
    Conquer, // Move troops into a newly conquered territory
    Fortify, // Relocate troops between connected territories at turn end
    SkipPhase, // Skip optional sub-phases (e.g. optional card trade or optional attack)
    EndTurn // End active player's turn
}