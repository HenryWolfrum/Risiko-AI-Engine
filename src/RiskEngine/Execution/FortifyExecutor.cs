using RiskEngine.Mutation;

namespace RiskEngine.Execution;

/// <summary>
/// Executes the fortification phase of a player's turn.
/// Responsible for moving troops between connected friendly territories.
/// </summary>
public static class FortifyExecutor
{
    /// <summary>
    /// Executes the fortification phase.
    /// </summary>
    public static void Execute(ref GameState state, IRiskPlayer player, GameLayout layout,ref EngineRandom rng)
    {
        state.CurrentPhase = GamePhase.Fortify;


        var activePlayer = state.PlayerTurn;


        // Calculate the maximum possible number of fortification actions.
        // This prevents endless loops caused by invalid player behaviour.
        var ownedTerritoriesCount = (byte)GameStateHelper.GetOwnedTerritoryCount(in state, activePlayer);

        // Limit the number of actions per phase to prevent infinite loops.
        // This is an execution safety limit, not a game rule.
        var maxFortifyMoves = (ownedTerritoriesCount * (ownedTerritoriesCount - 1)) >> 1;


        var moveCounter = 0;


        while (moveCounter < maxFortifyMoves)
        {
            // Ask player for a fortification action
            var action = player.DecideAction(in state, GamePhase.Fortify, layout);


            // Player chooses to end fortification phase
            if (action.Type == ActionType.SkipPhase || action.Type == ActionType.EndTurn)
            {
                break;
            }
            

            // Validate troop movement and connectivity rules
            var validation = RuleValidator.Validate(in state, in action, layout);


            if (!validation.IsValid)
            {
                // Invalid action ends the phase.
                // This prevents invalid players from blocking the simulation.
                break;
            }


            // Apply troop movement
            GameStateMutator.Apply(ref state, in action, ref rng,layout);


            moveCounter++;
        }
    }
}