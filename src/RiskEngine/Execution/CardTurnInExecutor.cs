using RiskEngine.State.Mutation;

namespace RiskEngine.State.Execution;

/// <summary>
/// Executes the card turn-in phase.
/// Handles mandatory and optional card exchanges.
/// </summary>
public static class CardTurnInExecutor
{
    /// <summary>
    /// Executes the card exchange phase for the current player.
    /// </summary>
    public static void Execute(ref GameState state, IRiskPlayer player, GameLayout layout, ref EngineRandom rng)
    {
        state.CurrentPhase = GamePhase.CardTurnIn;


        // Players with too many cards must trade in immediately.
        ExecuteMandatoryTradeIn(ref state, player, layout, ref rng);
        
        
        // Players may optionally trade in a valid set.
        ExecuteOptionalTradeIn(ref state, player, layout, ref rng);
    }


    /// <summary>
    /// Forces card exchanges while the player exceeds the maximum hand size.
    /// </summary>
    private static void ExecuteMandatoryTradeIn(ref GameState state, IRiskPlayer player, GameLayout layout, ref EngineRandom rng)
    {
        while (CardHelper.GetPlayerCardCount(in state, state.PlayerTurn) >= EngineConstants.FORCE_TRADE_CARD_COUNT)
        {
            // Ask player which card set should be exchanged.
            var action = player.DecideAction(in state, GamePhase.CardTurnIn, layout);


            // Validate action through the central validation system.
            var validation = RuleValidator.Validate(in state, in action, layout);


            if (validation.IsValid)
            {
                // Apply valid trade.
                GameStateMutator.Apply(ref state, in action, ref rng,layout);
            }
            else
            {
                // Invalid player action:
                // Use the first available legal card combination.
                var fallbackAction = CardHelper.FindFirstValidSet(in state, state.PlayerTurn, layout.Deck);
                
                GameStateMutator.Apply(ref state, in fallbackAction, ref rng,layout);
            }
        }
    }


    /// <summary>
    /// Allows the player to voluntarily exchange a valid card set.
    /// </summary>
    private static void ExecuteOptionalTradeIn(ref GameState state, IRiskPlayer player, GameLayout layout, ref EngineRandom rng)
    {
        // No valid card set available.
        if (!CardHelper.HasValidSet(in state, state.PlayerTurn, layout.Deck))
        {
            return;
        }
        
        // Ask player whether a voluntary trade should happen.
        var action = player.DecideAction(in state, GamePhase.CardTurnIn, layout);
        
        // Only trade actions are relevant here.
        if (action.Type != ActionType.TurnInCards)
        {
            return;
        }
        
        // Validate chosen card set.
        var validation = RuleValidator.Validate(in state, in action, layout);
        
        if (!validation.IsValid)
        {
            return;
        }
        
        // Apply valid voluntary trade.
        GameStateMutator.Apply(ref state, in action, ref rng,layout);
    }
}