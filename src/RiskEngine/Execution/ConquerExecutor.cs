using RiskEngine.Exceptions;
using RiskEngine.Mission;
using RiskEngine.Observer;
using RiskEngine.State.Mutation;
using RiskEngine.State.Validation;

namespace RiskEngine.State.Execution;

/// <summary>
/// Executes the conquest phase after a successful attack.
/// Handles troop movement into conquered territory, card trades, post-conquest reinforcement, and player elimination.
/// </summary>
public static class ConquerExecutor
{
    /// <summary>
    /// Executes the conquest phase.
    /// The attacker moves troops from the attacking territory into the newly conquered territory.
    /// </summary>
    public static void Execute(ref GameState state, IRiskPlayer attacker, byte defenderId, GameLayout layout, ref EngineRandom rng, IGameObserver? observer = null)
    {
        state.CurrentPhase = GamePhase.Conquer;

        // 1. Get and validate conquest move action
        var conquerAction = attacker.DecideAction(in state, layout);

        var validation = RuleValidator.Validate(in state, in conquerAction, layout);
        if (!validation.IsValid)
        {
            var sourceTroops = GameStateHelper.GetTerritoryTroops(in state, conquerAction.SourceTerritory);
            var targetTroops = GameStateHelper.GetTerritoryTroops(in state, conquerAction.TargetTerritory);

            throw new InvalidGameActionException(
                $"Invalid conquer action for Player {state.PlayerTurn}!\n" +
                $"  • Error Reason:      {validation.Error}\n" +
                $"  • Source Territory:  #{conquerAction.SourceTerritory} (Troops available: {sourceTroops})\n" +
                $"  • Target Territory:  #{conquerAction.TargetTerritory} (Current troops: {targetTroops})\n" +
                $"  • Requested Move:    {conquerAction.TroopCount} troops\n"
            );
        }

        // 2. Apply movement and record state
        GameStateMutator.Apply(ref state, in conquerAction, ref rng, layout);
        observer?.Record(in state, conquerAction);

        // 3. Process potential player elimination and card transfer
        CheckPlayerElimination(ref state, defenderId,observer);

        // 4. Check for game-winning condition
        if (MissionEvaluator.CheckEliminationWin(in state, in layout, state.PlayerTurn, out byte winnerId))
        {
            GameStateHelper.Terminate(ref state, winnerId);
            return;
        }

        // 5. Handle mandatory card exchange if hand limit is exceeded
        if (CardHelper.GetPlayerCardCount(in state, state.PlayerTurn) >= EngineConstants.FORCE_TRADE_CARD_COUNT)
        {
            CardTurnInExecutor.ExecuteMandatoryTradeIn(ref state, attacker, layout, ref rng, observer);
        }

        // 6. Place any newly acquired reinforcement troops from card sets
        if (GameStateHelper.GetPlayerTroopsToPlace(in state, state.PlayerTurn) > 0)
        {
            state.CurrentPhase = GamePhase.Reinforce;
            ReinforceExecutor.Execute(ref state, attacker, layout, ref rng, observer);
        }
        
    }

    /// <summary>
    /// Removes eliminated players and transfers their cards.
    /// </summary>
    private static void CheckPlayerElimination(ref GameState state, byte defenderId,IGameObserver? observer)
    {
        if (GameStateHelper.GetOwnedTerritoryCount(in state, defenderId) > 0)
        {
            return;
        }

        var attackerId = state.PlayerTurn;
        CardHelper.EliminateAndTransferCards(ref state, attackerId, defenderId);
        //Record State before card set Selection for new received cards
        observer?.Record(in state, null);

    }
}