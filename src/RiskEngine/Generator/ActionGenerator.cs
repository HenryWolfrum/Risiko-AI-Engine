namespace RiskEngine.State.Generation;

public static class ActionGenerator
{
    /// <summary>
    /// Generates all legal actions for the current game phase.
    /// The caller provides the action buffer to avoid allocations.
    /// </summary>
    public static int Generate(in GameState state, GameLayout layout, Span<GameAction> actions)
    {
        return state.CurrentPhase switch
        {
            // Generate all possible card trade-in actions
            GamePhase.CardTurnIn => CardTurnInActionGenerator.Generate(in state, layout, actions),

            GamePhase.Reinforce => ReinforcementActionGenerator.Generate(in state, actions),
            // Add further action generators here:
            // GamePhase.Attack => AttackActionGenerator.Generate(...),
            GamePhase.Fortify => FortifyActionGenerator.Generate(in state,layout, actions),

            _ => throw new ArgumentOutOfRangeException(nameof(state.CurrentPhase), state.CurrentPhase, "Unsupported game phase for action generation.")
        };
    }
}