using RiskEngine.State.Mutation;
using RiskEngine.State.Validation;

namespace RiskEngine.State.Execution;


/// <summary>
/// Executes the reinforcement phase of a player's turn.
/// Responsible for calculating available troops and applying placement actions.
/// </summary>
public static class ReinforceExecutor
{
    /// <summary>
    /// Executes the reinforcement phase.
    /// </summary>
    public static void Execute(ref GameState state, IRiskPlayer player, GameLayout layout, ref EngineRandom rng)
    {
        state.CurrentPhase = GamePhase.Reinforce;


        // Calculate reinforcement troops based on current game state
        var totalTroops = ReinforcementCalculator.CalculateTroops(in state, layout.Map);


        // Store available troops for the current player
        GameStateHelper.SetPlayerTroopsToPlace(ref state, state.PlayerTurn, totalTroops);


        // Limit the number of placement attempts.
        // This prevents endless loops caused by invalid player behaviour.
        var maxAttempts = GameStateHelper.GetOwnedTerritoryCount(in state, state.PlayerTurn);


        for (var i = 0; i < maxAttempts; i++)
        {
            // Stop when all reinforcement troops have been placed
            if (GameStateHelper.GetPlayerTroopsToPlace(in state, state.PlayerTurn) == 0)
            {
                break;
            }


            // Ask player where to place troops
            var action = player.DecideAction(in state, GamePhase.Reinforce, layout);


            // Validate reinforcement action
            var validation = RuleValidator.Validate(in state,in action, layout);


            if (validation.IsValid)
            {
                // Apply valid reinforcement placement
                GameStateMutator.Apply(ref state, in action, ref rng,layout);
            }
            else
            {
                // Invalid action:
                // Use fallback to guarantee valid state progression
                ApplyReinforceFallback(ref state, ref rng,layout);

                return;
            }
        }


        // Safety fallback:
        // Handle remaining troops if the player did not place everything
        if (GameStateHelper.GetPlayerTroopsToPlace(in state, state.PlayerTurn) > 0)
        {
            ApplyReinforceFallback(ref state, ref rng,layout);
        }
    }


    /// <summary>
    /// Places all remaining reinforcement troops on the first owned territory.
    /// Used as a safety mechanism for invalid player behaviour.
    /// </summary>
    private static void ApplyReinforceFallback(ref GameState state, ref EngineRandom rng,GameLayout layout)
    {
        // Select a guaranteed owned territory
        var fallbackTerritory = GameStateHelper.GetFirstTerritoryOwnedBy(in state, state.PlayerTurn);


        // Get all remaining troops
        var remainingTroops = GameStateHelper.GetPlayerTroopsToPlace(in state, state.PlayerTurn);


        var fallbackAction = new GameAction
        {
            Type = ActionType.Reinforce,
            TargetTerritory = fallbackTerritory,
            TroopCount = remainingTroops
        };


        // Apply fallback placement
        GameStateMutator.Apply(ref state, in fallbackAction, ref rng,layout);
    }
}