using System;
using RiskEngine.Exceptions;
using RiskEngine.Observer;
using RiskEngine.State.Mutation;
using RiskEngine.State.Validation;

namespace RiskEngine.State.Execution;

/// <summary>
/// Executes the reinforcement phase of a player's turn.
/// Responsible for calculating available troops, enforcing strict rule validation, and applying placement actions.
/// </summary>
public static class ReinforceExecutor
{
    /// <summary>
    /// Executes the reinforcement phase for the current player.
    /// </summary>
    public static void Execute(
        ref GameState state,
        IRiskPlayer player,
        GameLayout layout,
        ref EngineRandom rng,
        IGameObserver? observer = null)
    {
        state.CurrentPhase = GamePhase.Reinforce;

        // Calculate reinforcement troops based on current game state.
        var totalTroops = ReinforcementCalculator.CalculateTroops(
            in state,
            layout.Map);

        // Store available troops for the current player.
        GameStateHelper.SetPlayerTroopsToPlace(
            ref state,
            state.PlayerTurn,
            totalTroops);

        // Upper bound safety limit based on total troops to place.
        // Each valid action places at least 1 troop.
        var maxIterations = totalTroops + 10;
        var iterationCount = 0;

        while (GameStateHelper.GetPlayerTroopsToPlace(in state, state.PlayerTurn) > 0)
        {
            // Guard against infinite loops caused by stuck bots or state mutation bugs.
            if (++iterationCount > maxIterations)
            {
                var remaining = GameStateHelper.GetPlayerTroopsToPlace(
                    in state,
                    state.PlayerTurn);

                throw new InvalidGameActionException(
                    $"Reinforcement phase exceeded maximum iteration limit ({maxIterations}) for Player {state.PlayerTurn}!\n" +
                    $"  • Initial Troops to Place: {totalTroops}\n" +
                    $"  • Remaining Troops:         {remaining}"
                );
            }

            // Ask player where to place troops.
            var action = player.DecideAction(in state, layout);

            // Fail-fast: Reinforce phase strictly requires a Reinforce action type.
            if (action.Type != ActionType.Reinforce)
            {
                throw new InvalidGameActionException(
                    $"Player {state.PlayerTurn} submitted an invalid action type during reinforcement phase!\n" +
                    $"  • Expected: {ActionType.Reinforce}\n" +
                    $"  • Actual:   {action.Type}"
                );
            }

            // Validate reinforcement action through central rule system.
            var validation = RuleValidator.Validate(
                in state,
                in action,
                layout);

            if (!validation.IsValid)
            {
                var remainingTroops = GameStateHelper.GetPlayerTroopsToPlace(
                    in state,
                    state.PlayerTurn);

                var targetOwner = GameStateHelper.GetTerritoryOwner(
                    in state,
                    action.TargetTerritory);

                var currentTargetTroops = GameStateHelper.GetTerritoryTroops(
                    in state,
                    action.TargetTerritory);

                throw new InvalidGameActionException(
                    $"Invalid reinforcement action for Player {state.PlayerTurn}!\n" +
                    $"  • Error Reason:        {validation.Error}\n" +
                    $"  • Action Type:         {action.Type}\n" +
                    $"  • Target Territory:    #{action.TargetTerritory} (Owner: Player {targetOwner}, Current troops: {currentTargetTroops})\n" +
                    $"  • Requested Placement: {action.TroopCount} troops\n" +
                    $"  • Remaining to Place:  {remainingTroops} troops"
                );
            }

          
            
            // Apply valid reinforcement placement.
            GameStateMutator.Apply(ref state, in action, ref rng, layout);
            
            //Record
            observer?.Record(in state, action);

        }
    }
}