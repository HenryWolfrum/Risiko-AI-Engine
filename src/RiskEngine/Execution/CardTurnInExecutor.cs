using System;
using RiskEngine.Exceptions;
using RiskEngine.Observer;
using RiskEngine.State.Mutation;
using RiskEngine.State.Validation;

namespace RiskEngine.State.Execution;

/// <summary>
/// Executes the card turn-in phase.
/// Handles mandatory and optional card exchanges with strict safety guards.
/// </summary>
public static class CardTurnInExecutor
{
    /// <summary>
    /// Maximum allowed trade iterations per phase to prevent infinite loops.
    /// </summary>
    private const int MAX_TRADE_ITERATIONS = 10;

    /// <summary>
    /// Executes the card exchange phase for the current player.
    /// </summary>
    public static void Execute(
        ref GameState state,
        IRiskPlayer player,
        GameLayout layout,
        ref EngineRandom rng,
        IGameObserver? observer = null)
    {
        state.CurrentPhase = GamePhase.CardTurnIn;

        // Players with too many cards must trade in immediately.
        ExecuteMandatoryTradeIn(ref state, player, layout, ref rng, observer);

        // Players may optionally trade in additional valid sets.
        ExecuteOptionalTradeIn(ref state, player, layout, ref rng, observer);
    }


    /// <summary>
    /// Forces card exchanges while the player exceeds the maximum hand size.
    /// </summary>
    private static void ExecuteMandatoryTradeIn(
        ref GameState state,
        IRiskPlayer player,
        GameLayout layout,
        ref EngineRandom rng,
        IGameObserver? observer = null)
    {
        var iterationCount = 0;

        while (CardHelper.GetPlayerCardCount(in state, state.PlayerTurn) >= EngineConstants.FORCE_TRADE_CARD_COUNT)
        {
            if (++iterationCount > MAX_TRADE_ITERATIONS)
            {
                throw new InvalidGameActionException(
                    $"Mandatory card trade exceeded maximum iteration limit ({MAX_TRADE_ITERATIONS}) for Player {state.PlayerTurn}!\n" +
                    $"  • Hand Size: {CardHelper.GetPlayerCardCount(in state, state.PlayerTurn)} cards\n" +
                    $"  • Threshold: {EngineConstants.FORCE_TRADE_CARD_COUNT} cards"
                );
            }


            // Ask player which card set should be exchanged.
            var action = player.DecideAction(in state, layout);


            if (action.Type != ActionType.TurnInCards)
            {
                throw new InvalidOperationException(
                    $"Player {state.PlayerTurn} submitted an invalid action type during mandatory card trade!\n" +
                    $"  • Expected: {ActionType.TurnInCards}\n" +
                    $"  • Actual:   {action.Type}"
                );
            }


            var validation = RuleValidator.Validate(in state, in action, layout);

            if (!validation.IsValid)
            {
                throw new InvalidGameActionException(
                    $"Invalid mandatory card trade action for Player {state.PlayerTurn}!\n" +
                    $"  • Error Reason:  {validation.Error}\n" +
                    $"  • Action Type:   {action.Type}\n" +
                    $"  • Submitted Cards:\n" +
                    $"      1. Card ID: {action.Card1}\n" +
                    $"      2. Card ID: {action.Card2}\n" +
                    $"      3. Card ID: {action.Card3}"
                );
            }


            // Apply card exchange.
            GameStateMutator.Apply(
                ref state,
                in action,
                ref rng,
                layout);


            // Record completed event after mutation.
            observer?.Record(
                in state,
                in action);
        }
    }


    /// <summary>
    /// Allows the player to voluntarily exchange valid card sets.
    /// </summary>
    private static void ExecuteOptionalTradeIn(
        ref GameState state,
        IRiskPlayer player,
        GameLayout layout,
        ref EngineRandom rng,
        IGameObserver? observer = null)
    {
        if (CardHelper.HasValidSet(in state, state.PlayerTurn, layout.Deck))
        {
            var action = player.DecideAction(in state, layout);


            if (action.Type == ActionType.SkipPhase ||
                action.Type == ActionType.EndTurn)
            {
                return;
            }


            if (action.Type != ActionType.TurnInCards)
            {
                throw new InvalidGameActionException(
                    $"Player {state.PlayerTurn} submitted an unexpected action type during optional card trade!\n" +
                    $"  • Expected: {ActionType.TurnInCards}, {ActionType.SkipPhase}, or {ActionType.EndTurn}\n" +
                    $"  • Actual:   {action.Type}"
                );
            }


            var validation = RuleValidator.Validate(in state, in action, layout);

            if (!validation.IsValid)
            {
                throw new InvalidGameActionException(
                    $"Invalid voluntary card trade action for Player {state.PlayerTurn}!\n" +
                    $"  • Error Reason:  {validation.Error}\n" +
                    $"  • Action Type:   {action.Type}\n" +
                    $"  • Submitted Cards:\n" +
                    $"      1. Card ID: {action.Card1}\n" +
                    $"      2. Card ID: {action.Card2}\n" +
                    $"      3. Card ID: {action.Card3}"
                );
            }


            // Apply voluntary card exchange.
            GameStateMutator.Apply(
                ref state,
                in action,
                ref rng,
                layout);


            // Record completed event after mutation.
            observer?.Record(in state, in action);
        }
    }
}