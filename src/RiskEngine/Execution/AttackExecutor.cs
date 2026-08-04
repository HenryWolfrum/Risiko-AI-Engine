using System;
using RiskEngine.Exceptions;
using RiskEngine.Observer;
using RiskEngine.State.Mutation;
using RiskEngine.State.Validation;

namespace RiskEngine.State.Execution;

public static class AttackExecutor
{
    private const int MAX_ATTACK_ITERATIONS = 500;

    public static void Execute(
        ref GameState state,
        IRiskPlayer[] players,
        GameLayout layout,
        ref EngineRandom rng,
        IGameObserver? observer = null)
    {
        state.CurrentPhase = GamePhase.Attack;
        var conqueredTerritoryThisTurn = false;
        var iterationCount = 0;

        while (AttackHelper.CanPlayerAttack(in state, state.PlayerTurn, layout.Map))
        {
            if (++iterationCount > MAX_ATTACK_ITERATIONS)
            {
                throw new InvalidEngineStateException(
                    $"Attack phase exceeded maximum iteration limit ({MAX_ATTACK_ITERATIONS}) for Player {state.PlayerTurn}!"
                );
            }

            if (state.CurrentPhase == GamePhase.Terminated)
                return;

            var attackerPlayer = players[state.PlayerTurn];
            var attackAction = attackerPlayer.DecideAction(in state, layout);

            if (attackAction.Type == ActionType.SkipPhase ||
                attackAction.Type == ActionType.EndTurn)
            {
                break;
            }

            if (attackAction.Type != ActionType.Attack)
            {
                throw new InvalidGameActionException(
                    $"Player {state.PlayerTurn} submitted invalid action type {attackAction.Type} during attack phase!"
                );
            }

            var validation = RuleValidator.Validate(in state, in attackAction, layout);
            if (!validation.IsValid)
            {
                throw new InvalidGameActionException($"Invalid attack action: {validation.Error}");
            }

            state.AttackerTerritory = attackAction.SourceTerritory;
            state.DefenderTerritory = attackAction.TargetTerritory;

            var defenderTerritory = attackAction.TargetTerritory;
            var defenderPlayerId = GameStateHelper.GetTerritoryOwner(in state, defenderTerritory);
            var defenderPlayer = players[defenderPlayerId];

            state.CurrentPhase = GamePhase.Defend;
            var previousTurn = state.PlayerTurn;
            state.PlayerTurn = defenderPlayerId;

            var defenderAction = defenderPlayer.DecideAction(in state, layout);

            state.PlayerTurn = previousTurn;
            state.CurrentPhase = GamePhase.Attack;

            if (defenderAction.Type != ActionType.Defend ||
                !AttackRules.IsValidDefenderDice(in state, defenderTerritory, defenderAction.ChosenDefenderDiceCount))
            {
                throw new InvalidGameActionException("Invalid defender action!");
            }

            attackAction.ChosenDefenderDiceCount = defenderAction.ChosenDefenderDiceCount;

            // 1. Pre-Action Recording: Zustand VOR dem Kampf speichern
            observer?.Record(in state, in attackAction);

            // 2. Kampfergebnis anwenden
            GameStateMutator.Apply(ref state, in attackAction, ref rng, layout);

            // 3. Eroberung verarbeiten (falls Ziel auf 0 Truppen)
            if (GameStateHelper.GetTerritoryTroops(in state, defenderTerritory) == 0)
            {
                conqueredTerritoryThisTurn = true;
                ConquerExecutor.Execute(ref state, attackerPlayer, defenderPlayerId, layout, ref rng, observer);
            }

            if (state.CurrentPhase != GamePhase.Terminated)
                state.CurrentPhase = GamePhase.Attack;

            state.AttackerTerritory = EngineConstants.NO_VALUE;
            state.DefenderTerritory = EngineConstants.NO_VALUE;

            if (GameStateHelper.GetActivePlayerCount(in state) <= 1)
                break;
        }

        if (conqueredTerritoryThisTurn && state.CurrentPhase != GamePhase.Terminated)
        {
            CardHelper.GiveBonusCard(ref state, state.PlayerTurn, ref rng, layout.Deck);
        }
    }
}