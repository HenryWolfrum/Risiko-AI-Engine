using RiskEngine.State.Mutation;
using RiskEngine.State.Validation;

namespace RiskEngine.State.Execution;

/// <summary>
/// Executes the attack phase of a player's turn.
/// Handles attack decisions, combat resolution and conquest triggering.
/// </summary>
public static class AttackExecutor
{
    /// <summary>
    /// Executes the complete attack phase.
    /// </summary>
    public static void Execute(ref GameState state, IRiskPlayer[] players, GameLayout layout, ref EngineRandom rng)
    {
        state.CurrentPhase = GamePhase.Attack;


        // Tracks whether the player conquered at least one territory.
        // A conquered territory grants exactly one bonus card.
        var conqueredTerritoryThisTurn = false;


        // Continue attacking while at least one legal attack exists.
        while (AttackHelper.CanPlayerAttack(in state, state.PlayerTurn, layout.Map))
        {
            // After Attack Game could already be decided
            if (state.CurrentPhase == GamePhase.Terminated)
            {
                return;
            }
            
            var attackerPlayer = players[state.PlayerTurn];


            // Ask attacker for the next attack decision.
            var attackAction = attackerPlayer.DecideAction(in state, layout);


            // Player ends attack phase voluntarily.
            if (attackAction.Type == ActionType.SkipPhase || attackAction.Type == ActionType.EndTurn)
            {
                break;
            }


            // Validate attack action.
            var validation = RuleValidator.Validate(in state, in attackAction, layout);


            if (!validation.IsValid)
            {
                // Invalid player action ends the attack phase.
                break;
            }


            // Get defender.
            var defenderPlayerId = GameStateHelper.GetTerritoryOwner(in state, attackAction.TargetTerritory);


            var defenderPlayer = players[defenderPlayerId];


            // Ask defender how many dice should be used.
            var defenderDice = defenderPlayer.DecideAction(in state,layout).ChosenDefenderDiceCount;


            // Safety fallback for invalid defender behaviour.
            if (!AttackRules.IsValidDefenderDice(in state, attackAction.TargetTerritory, defenderDice))
            {
                defenderDice = AttackRules.GetMaxDefenderDice(in state, attackAction.TargetTerritory);
            }


            attackAction.ChosenDefenderDiceCount = defenderDice;


            // Resolve combat and apply losses.
            GameStateMutator.Apply(ref state, in attackAction, ref rng,layout);


            // Territory is conquered when defender troops reach zero.
            if (GameStateHelper.GetTerritoryTroops(in state, attackAction.TargetTerritory) == 0)
            {
                conqueredTerritoryThisTurn = true;


                ConquerExecutor.Execute(ref state, attackerPlayer, defenderPlayerId, in attackAction, layout, ref rng);


                // Stop if only one player remains.
                if (GameStateHelper.GetActivePlayerCount(in state) <= 1)
                {
                    break;
                }
            }
        }


        // Award one territory card after successful conquest.
        if (conqueredTerritoryThisTurn&&state.CurrentPhase != GamePhase.Terminated)
        {
            CardHelper.GiveBonusCard(ref state, state.PlayerTurn, ref rng,layout.Deck);
        }
    }
}