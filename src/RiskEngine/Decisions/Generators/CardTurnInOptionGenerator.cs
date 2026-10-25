using RiskEngine.Cards;
using RiskEngine.Constants;
using RiskEngine.Layout;
using RiskEngine.Rules;
using RiskEngine.State;

namespace RiskEngine.Decisions.Generators;

public static class CardTurnInOptionGenerator
{
    /// <summary>
    ///     Generates all valid card set turn-in options for the active player.
    ///     Uses stack-allocated spans to evaluate unique 3-card combinations without memory allocation.
    ///     If the player is not forced to trade, a phase skip option is appended.
    /// </summary>
    public static int Generate(in GameState state, GameLayout layout, Span<DecisionOption> options)
    {
        var player = GameStateHelper.GetPlayerOnTurn(in state);

        var hand = CardHelper.GetPlayerCardsBitboard(in state, player);

        Span<byte> cards = stackalloc byte[64];
        var cardCount = CardHelper.ExtractCardIds(hand, cards);

        Span<CardType> types = stackalloc CardType[64];

        // Cache card types to avoid repeated lookups during combination checks
        for (var i = 0; i < cardCount; i++) types[i] = layout.Deck.TerritoryToType[cards[i]];

        var optionCount = 0;

        // Generate all unique 3-card combinations
        for (var a = 0; a < cardCount - 2; a++)
        for (var b = a + 1; b < cardCount - 1; b++)
        for (var c = b + 1; c < cardCount; c++)
        {
            if (!TurnInCardsRules.IsValidSet(types[a], types[b], types[c])) continue;

            // Card turn-ins use all 3 value bytes (_value1, _value2, _value3) for Card IDs.
            // ParameterSpace is None since picking the set fully identifies the decision.
            options[optionCount++] = DecisionOption.CardTurnIn(
                cards[a],
                cards[b],
                cards[c]
            );
        }

        // Card trade is mandatory if the player exceeds the hand limit.
        // In this case, skipping card trade is not a valid option.
        var mustTrade = CardHelper.GetPlayerCardCount(in state, player) >= EngineConstants.FORCE_TRADE_CARD_COUNT;

        if (!mustTrade) options[optionCount++] = DecisionOption.SkipPhase();

        return optionCount;
    }
}