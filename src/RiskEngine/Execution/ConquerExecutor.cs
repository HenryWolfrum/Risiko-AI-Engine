using RiskEngine.Exceptions;
using RiskEngine.Mission;
using RiskEngine.Observer;
using RiskEngine.State.Mutation;

namespace RiskEngine.State.Execution;

/// <summary>
/// Executes the conquest phase after a successful attack.
/// Handles troop movement into conquered territory and player elimination.
/// </summary>
public static class ConquerExecutor
{
    /// <summary>
    /// Executes the conquest phase.
    /// The attacker moves troops from the attacking territory
    /// into the newly conquered territory.
    /// </summary>
    public static void Execute(
        ref GameState state,
        IRiskPlayer attacker,
        byte defenderId,
        GameLayout layout,
        ref EngineRandom rng,
        IGameObserver? observer = null)
    {
        state.CurrentPhase = GamePhase.Conquer;


        // Ask attacker how many troops should move
        // into the conquered territory.
        var conquerAction = attacker.DecideAction(in state, layout);


        // Validate conquest action through central rule system.
        var validation = RuleValidator.Validate(
            in state,
            in conquerAction,
            layout);


        if (!validation.IsValid)
        {
            var sourceTroops = GameStateHelper.GetTerritoryTroops(
                in state,
                conquerAction.SourceTerritory);

            var targetTroops = GameStateHelper.GetTerritoryTroops(
                in state,
                conquerAction.TargetTerritory);

            throw new InvalidGameActionException(
                $"Invalid conquer action for Player {state.PlayerTurn}!\n" +
                $"  • Error Reason:      {validation.Error}\n" +
                $"  • Source Territory:  #{conquerAction.SourceTerritory} (Troops available: {sourceTroops})\n" +
                $"  • Target Territory:  #{conquerAction.TargetTerritory} (Current troops: {targetTroops})\n" +
                $"  • Requested Move:    {conquerAction.TroopCount} troops\n"
            );
        }

        //Record
        observer?.Record(in state, conquerAction);

        // Apply troop movement and ownership transfer.
        GameStateMutator.Apply(ref state, in conquerAction, ref rng, layout);
        
        //Record
        observer?.Record(in state, conquerAction);


        // Check whether defender lost their final territory.
        CheckPlayerElimination(ref state, in layout, defenderId);


        //Check if any player has won
        if (MissionEvaluator.CheckEliminationWin(in state, in layout, state.PlayerTurn, out byte winnerId))
        {
            GameStateHelper.Terminate(ref state, winnerId);
        }

    }



    /// <summary>
    /// Removes eliminated players and transfers their cards.
    /// </summary>
    private static void CheckPlayerElimination(ref GameState state, in GameLayout layout, byte defenderId)
    {
        // Defender still owns territories.
        if (GameStateHelper.GetOwnedTerritoryCount(in state, defenderId) > 0)
        {
            return;
        }


        var attackerId = state.PlayerTurn;


        // Transfer cards and remove defender from active players.
        CardHelper.EliminateAndTransferCards(ref state, attackerId, defenderId);


        
    }
}