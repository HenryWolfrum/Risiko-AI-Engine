using RiskEngine.Actions;
using RiskEngine.Cards;
using RiskEngine.Constants;
using RiskEngine.Error;
using RiskEngine.Layout;
using RiskEngine.Mission;
using RiskEngine.Mutators;
using RiskEngine.Observer;
using RiskEngine.Player;
using RiskEngine.Random;
using RiskEngine.Rules;
using RiskEngine.State;

namespace RiskEngine.Execution;

/// <summary>
///     Executes the conquest phase after a successful attack.
///     Handles troop movement into conquered territory, card trades, post-conquest reinforcement, and player elimination.
/// </summary>
public static class ConquerExecutor
{
    /// <summary>
    ///     Executes the conquest phase.
    ///     The attacker moves troops from the attacking territory into the newly conquered territory.
    /// </summary>
    public static void Execute(ref GameState state, IRiskPlayer attacker, byte defenderId, GameLayout layout, ref EngineRandom rng, IGameObserver? observer = null)
    {
        state.CurrentPhase = GamePhase.Conquer;
        
        byte playerOnTurn = GameStateHelper.GetPlayerOnTurn(in state);

        // 1. Get and validate conquest move action
        var conquerAction = attacker.DecideAction(in state, layout);

        var validation = RuleValidator.Validate(in state, in conquerAction, layout);
        if (!validation.IsValid)
        {
            var sourceTroops = GameStateHelper.GetTerritoryTroops(in state, conquerAction.SourceTerritory);
            var targetTroops = GameStateHelper.GetTerritoryTroops(in state, conquerAction.TargetTerritory);

            throw new InvalidGameActionException(
                $"Invalid conquer action for Player {playerOnTurn}!\n" +
                $"  • Error Reason:      {validation.Error}\n" +
                $"  • Source Territory:  #{conquerAction.SourceTerritory} (Troops available: {sourceTroops})\n" +
                $"  • Target Territory:  #{conquerAction.TargetTerritory} (Current troops: {targetTroops})\n" +
                $"  • Requested Move:    {conquerAction.TroopCount} troops\n"
            );
        }

        // 2. Apply movement and record state
        GameStateMutator.Apply(ref state, in conquerAction, ref rng, layout,observer);

        // 3. Process potential player elimination and card transfer
        CheckPlayerElimination(ref state, defenderId);

        // 4. Check for game-winning condition
        if (MissionEvaluator.CheckEliminationWin(in state, in layout, playerOnTurn, out byte winnerId))
        {
            GameStateHelper.Terminate(ref state, winnerId);
            return;
        }

        // 5. Handle mandatory card exchange if hand limit is exceeded
        if (CardHelper.GetPlayerCardCount(in state, playerOnTurn) >= EngineConstants.FORCE_TRADE_CARD_COUNT)
            CardTurnInExecutor.ExecuteMandatoryTradeIn(ref state, attacker, layout, ref rng, observer);

        // 6. Place any newly acquired reinforcement troops from card sets
        if (GameStateHelper.GetPlayerTroopsToPlace(in state, playerOnTurn) > 0)
        {
            state.CurrentPhase = GamePhase.Reinforce;
            ReinforceExecutor.Execute(ref state, attacker, layout, ref rng, observer);
        }
    }

    /// <summary>
    ///     Removes eliminated players and transfers their cards.
    /// </summary>
    private static void CheckPlayerElimination(ref GameState state, byte defenderId)
    {
        if (GameStateHelper.GetOwnedTerritoryCount(in state, defenderId) > 0) return;

        var attackerId = GameStateHelper.GetPlayerOnTurn(in state);
        
        CardHelper.TransferCards(ref state, defenderId, attackerId);
        GameStateHelper.EliminatePlayer(ref state, defenderId);
        
    }
}