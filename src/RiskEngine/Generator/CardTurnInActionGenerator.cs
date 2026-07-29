using RiskEngine.State.Rules;

namespace RiskEngine.State.Generation;

public static class CardTurnInActionGenerator
{
    public static int Generate(in GameState state, byte player, GameLayout layout, Span<GameAction> actions)
    {
        ulong hand = CardHelper.GetPlayerCardsBitboard(state, player);

        
        

        Span<byte> cards = stackalloc byte[64];

        int cardCount = CardHelper.ExtractCardIds(hand, cards);

        
        Span<CardType> types = stackalloc CardType[64];

        for (int i = 0; i < cardCount; i++)
        {
            types[i] = layout.Deck.TerritoryToType[cards[i]];
        }

        int actionCount = 0;


        for (int a = 0; a < cardCount - 2; a++)
        {
            for (int b = a + 1; b < cardCount - 1; b++)
            {
                for (int c = b + 1; c < cardCount; c++)
                {
                    byte card1 = cards[a];
                    byte card2 = cards[b];
                    byte card3 = cards[c];


                    if (!TurnInCardsRules.IsValidSet(types[a], types[b], types[c]))
                    {
                        continue;
                    }


                    actions[actionCount++] = new GameAction
                        {
                            Type = ActionType.TurnInCards,
                            Card1 = card1,
                            Card2 = card2,
                            Card3 = card3
                        };
                }
            }
        }


        return actionCount;
    }
}