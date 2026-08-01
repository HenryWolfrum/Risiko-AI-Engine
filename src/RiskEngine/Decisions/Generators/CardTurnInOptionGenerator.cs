using System;
using RiskEngine.Decisions; 
using RiskEngine.State.Rules;

namespace RiskEngine.State.Generation;

public static class CardTurnInOptionGenerator
{
    /// <summary>
    /// Generates all valid card set turn-in options for the active player.
    /// Uses stack-allocated spans to evaluate unique 3-card combinations without memory allocation.
    /// If the player is not forced to trade, a phase skip option is appended.
    /// </summary>
    public static int Generate(in GameState state, GameLayout layout, Span<DecisionOption> options)
    {
        byte player = state.PlayerTurn;
        
        ulong hand = CardHelper.GetPlayerCardsBitboard(in state, player);

        Span<byte> cards = stackalloc byte[64];
        int cardCount = CardHelper.ExtractCardIds(hand, cards);

        Span<CardType> types = stackalloc CardType[64];

        // Cache card types to avoid repeated lookups during combination checks
        for (int i = 0; i < cardCount; i++)
        {
            types[i] = layout.Deck.TerritoryToType[cards[i]];
        }

        int optionCount = 0;

        // Generate all unique 3-card combinations
        for (int a = 0; a < cardCount - 2; a++)
        {
            for (int b = a + 1; b < cardCount - 1; b++)
            {
                for (int c = b + 1; c < cardCount; c++)
                {
                    if (!TurnInCardsRules.IsValidSet(types[a], types[b], types[c]))
                    {
                        continue;
                    }

                    // Card turn-ins use all 3 value bytes (_value1, _value2, _value3) for Card IDs.
                    // ParameterSpace is None since picking the set fully identifies the decision.
                    options[optionCount++] = DecisionOption.CardTurnIn(
                        card1: cards[a],
                        card2: cards[b],
                        card3: cards[c]
                    );
                }
            }
        }

        // Card trade is mandatory if the player exceeds the hand limit.
        // In this case, skipping card trade is not a valid option.
        bool mustTrade = CardHelper.GetPlayerCardCount(in state, player) >= EngineConstants.FORCE_TRADE_CARD_COUNT;

        if (!mustTrade)
        {
            options[optionCount++] = DecisionOption.SkipPhase();
        }

        return optionCount;
    }
}