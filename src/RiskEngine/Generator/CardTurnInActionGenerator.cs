using RiskEngine.State.Rules;

namespace RiskEngine.State.Generation;

public static class CardTurnInActionGenerator
{
    public static int Generate(in GameState state, GameLayout layout, Span<GameAction> actions)
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


        int actionCount = 0;


        // Generate all unique 3-card combinations
        for (int a = 0; a < cardCount - 2; a++)
        {
            for (int b = a + 1; b < cardCount - 1; b++)
            {
                for (int c = b + 1; c < cardCount; c++)
                {
                    if (!TurnInCardsRules.IsValidSet(
                            types[a],
                            types[b],
                            types[c]))
                    {
                        continue;
                    }
                    
                    actions[actionCount++] = new GameAction
                    {
                        Type = ActionType.TurnInCards,
                        Card1 = cards[a],
                        Card2 = cards[b],
                        Card3 = cards[c]
                    };
                }
            }
        }


        // Card trade is mandatory if the player exceeds the hand limit.
        // In this case skipping card trade is not allowed.
        bool mustTrade = CardHelper.GetPlayerCardCount(in state, player) >= EngineConstants.FORCE_TRADE_CARD_COUNT;


        if (!mustTrade)
        {
            actions[actionCount++] = new GameAction
            {
                Type = ActionType.SkipPhase
            };
        }


        return actionCount;
    }
}