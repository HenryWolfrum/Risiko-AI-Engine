using System;
using RiskEngine.Exceptions;
using RiskEngine.State.Mutation;
using RiskEngine.State.Validation;

namespace RiskEngine.State.Execution;

/// <summary>
/// Executes the attack phase of a player's turn.
/// Handles attack decisions, combat resolution, and conquest triggering with strict safety guards.
/// </summary>
public static class AttackExecutor
{
    /// <summary>
    /// Maximum allowed attack iterations per phase to prevent infinite loops.
    /// </summary>
    private const int MAX_ATTACK_ITERATIONS = 500;

    /// <summary>
    /// Executes the complete attack phase for the current player.
    /// </summary>
    public static void Execute(ref GameState state, IRiskPlayer[] players, GameLayout layout, ref EngineRandom rng)
    {
        state.CurrentPhase = GamePhase.Attack;
        var conqueredTerritoryThisTurn = false;
        var iterationCount = 0;

        // Attacking is not limited as long as valid attacks are possible.
        while (AttackHelper.CanPlayerAttack(in state, state.PlayerTurn, layout.Map))
        {
            // Upper bound safety check to catch infinite loops or stuck AI decision loops.
            if (++iterationCount > MAX_ATTACK_ITERATIONS)
            {
                throw new InvalidEngineStateException(
                    $"Attack phase exceeded maximum iteration limit ({MAX_ATTACK_ITERATIONS}) for Player {state.PlayerTurn}!"
                );
            }

            // After conquer or previous combat, game could already be terminated.
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

            // Fail-fast: Action type must strictly be Attack when not skipping.
            if (attackAction.Type != ActionType.Attack)
            {
                throw new InvalidGameActionException(
                    $"Player {state.PlayerTurn} submitted an invalid action type during attack phase!\n" +
                    $"  • Expected: {ActionType.Attack}, {ActionType.SkipPhase}, or {ActionType.EndTurn}\n" +
                    $"  • Actual:   {attackAction.Type}"
                );
            }

            // 1. Validate action FIRST before mutating state cache.
            var validation = RuleValidator.Validate(in state, in attackAction, layout);

            if (!validation.IsValid)
            {
                var sourceTroops = GameStateHelper.GetTerritoryTroops(in state, attackAction.SourceTerritory);
                var targetTroops = GameStateHelper.GetTerritoryTroops(in state, attackAction.TargetTerritory);
                var targetOwner = GameStateHelper.GetTerritoryOwner(in state, attackAction.TargetTerritory);
                var maxAllowedDice = (byte)Math.Min(3, Math.Max(0, sourceTroops - 1));

                throw new InvalidGameActionException(
                    $"Invalid attack action for Player {state.PlayerTurn}!\n" +
                    $"  • Error Reason:        {validation.Error}\n" +
                    $"  • Action Type:         {attackAction.Type}\n" +
                    $"  • Source Territory:    #{attackAction.SourceTerritory} (Troops available: {sourceTroops})\n" +
                    $"  • Target Territory:    #{attackAction.TargetTerritory} (Troops: {targetTroops}, Owner: Player {targetOwner})\n" +
                    $"  • Requested Dice:      {attackAction.ChosenAttackerDiceCount} (Max allowed: {maxAllowedDice})"
                );
            }

            // 2. Set state cache ONLY after action validation passes.
            state.AttackerTerritory = attackAction.SourceTerritory;
            state.DefenderTerritory = attackAction.TargetTerritory;

            var defenderTerritory = attackAction.TargetTerritory;
            var defenderPlayerId = GameStateHelper.GetTerritoryOwner(in state, defenderTerritory);
            var defenderPlayer = players[defenderPlayerId];

            // Ask defender how many dice to defend with.
            state.CurrentPhase = GamePhase.Defend;
            var previousTurn = state.PlayerTurn;
            state.PlayerTurn = defenderPlayerId;

            var defenderAction = defenderPlayer.DecideAction(in state, layout);

            // Restore turn and phase state immediately after defender query.
            state.PlayerTurn = previousTurn;
            state.CurrentPhase = GamePhase.Attack;

            // Fail-fast: Defender action validation.
            if (defenderAction.Type != ActionType.Defend)
            {
                throw new InvalidGameActionException(
                    $"Player {defenderPlayerId} submitted an invalid action type during defense!\n" +
                    $"  • Expected: {ActionType.Defend}\n" +
                    $"  • Actual:   {defenderAction.Type}"
                );
            }

            byte defenderDice = defenderAction.ChosenDefenderDiceCount;

            if (!AttackRules.IsValidDefenderDice(in state, defenderTerritory, defenderDice))
            {
                var maxDefenderDice = AttackRules.GetMaxDefenderDice(in state, defenderTerritory);
                var defenderTroops = GameStateHelper.GetTerritoryTroops(in state, defenderTerritory);

                throw new InvalidGameActionException(
                    $"Player {defenderPlayerId} selected an invalid defender dice count!\n" +
                    $"  • Target Territory: #{defenderTerritory} (Troops: {defenderTroops})\n" +
                    $"  • Requested Dice:   {defenderDice}\n" +
                    $"  • Max Allowed:      {maxDefenderDice}"
                );
            }

            attackAction.ChosenDefenderDiceCount = defenderDice;

            // Resolve combat and apply troop losses.
            GameStateMutator.Apply(ref state, in attackAction, ref rng, layout);

            // Territory is conquered when defender troops reach zero.
            if (GameStateHelper.GetTerritoryTroops(in state, defenderTerritory) == 0)
            {
                conqueredTerritoryThisTurn = true;

                ConquerExecutor.Execute(ref state, attackerPlayer, defenderPlayerId, layout, ref rng);

                // Attack Phase only continues if game was not decided after conquering.
                if (state.CurrentPhase != GamePhase.Terminated)
                {
                    state.CurrentPhase = GamePhase.Attack;
                }

                // Stop execution if only one player remains (game decided).
                if (GameStateHelper.GetActivePlayerCount(in state) <= 1)
                {
                    // Clean up cache before exiting on game end.
                    state.AttackerTerritory = EngineConstants.NO_VALUE;
                    state.DefenderTerritory = EngineConstants.NO_VALUE;
                    break;
                }
            }

            // Direct cache cleanup at the end of each combat iteration.
            state.AttackerTerritory = EngineConstants.NO_VALUE;
            state.DefenderTerritory = EngineConstants.NO_VALUE;
        }

        // Award one territory card after at least one successful conquest during this turn.
        if (conqueredTerritoryThisTurn && state.CurrentPhase != GamePhase.Terminated)
        {
            CardHelper.GiveBonusCard(ref state, state.PlayerTurn, ref rng, layout.Deck);
        }
    }
}