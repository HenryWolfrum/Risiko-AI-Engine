using System;
using RiskEngine.Exceptions;
using RiskEngine.State.Mutation;
using RiskEngine.State.Validation;

namespace RiskEngine.State.Execution;

/// <summary>
/// Executes the fortification phase of a player's turn.
/// Responsible for moving troops between connected friendly territories.
/// </summary>
public static class FortifyExecutor
{
    /// <summary>
    /// Executes the fortification phase.
    /// </summary>
    public static void Execute(ref GameState state, IRiskPlayer player, GameLayout layout, ref EngineRandom rng)
    {
        state.CurrentPhase = GamePhase.Fortify;

        // Limit the number of actions per phase to prevent infinite loops.
        // This is an execution safety limit, not a game rule.
        var maxFortifyMoves = EngineConstants.MAX_FORTIFY_MOVES_PER_TURN;
        var moveCounter = 0;

        while (true)
        {
            // Safety upper-bound check against infinite loops / stuck AI decision loops.
            if (moveCounter >= maxFortifyMoves)
            {
                throw new InvalidEngineStateException(
                    $"Fortify phase exceeded maximum allowed moves limit ({maxFortifyMoves}) for Player {state.PlayerTurn}!"
                );
            }

            // Ask player for a fortification action
            var action = player.DecideAction(in state, layout);

            // Player chooses to end fortification phase
            if (action.Type == ActionType.SkipPhase || action.Type == ActionType.EndTurn)
            {
                break;
            }

            // Fail-fast: Action type must strictly be Fortify when not skipping/ending
            if (action.Type != ActionType.Fortify)
            {
                throw new InvalidGameActionException(
                    $"Player {state.PlayerTurn} submitted an invalid action type during fortify phase!\n" +
                    $"  • Expected: {ActionType.Fortify}, {ActionType.SkipPhase}, or {ActionType.EndTurn}\n" +
                    $"  • Actual:   {action.Type}"
                );
            }

            // Validate troop movement and connectivity rules
            var validation = RuleValidator.Validate(in state, in action, layout);

            if (!validation.IsValid)
            {
                var sourceTroops = GameStateHelper.GetTerritoryTroops(in state, action.SourceTerritory);
                var targetTroops = GameStateHelper.GetTerritoryTroops(in state, action.TargetTerritory);

                throw new InvalidGameActionException(
                    $"Invalid fortify action for Player {state.PlayerTurn}!\n" +
                    $"  • Error Reason:     {validation.Error}\n" +
                    $"  • Source Territory: #{action.SourceTerritory} (Troops: {sourceTroops})\n" +
                    $"  • Target Territory: #{action.TargetTerritory} (Troops: {targetTroops})\n" +
                    $"  • Requested Troops: {action.TroopCount}"
                );
            }

            // Apply troop movement
            GameStateMutator.Apply(ref state, in action, ref rng, layout);

            moveCounter++;
        }
    }
}