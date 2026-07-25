namespace RiskEngine.Mutation;

public static unsafe class CardTurnInMutator
{
    // Trades three cards for reinforcement troops
    public static void Apply(
        ref GameState state,
        in GameAction action)
    {
        byte player = state.PlayerTurn;

        // Remove cards from player's hand
        RemoveCard(ref state, player, action.Card1);
        RemoveCard(ref state, player, action.Card2);
        RemoveCard(ref state, player, action.Card3);

        // Increase traded set counter
        state.CardSetsTradedCount++;

        // Add reinforcement bonus
        state.PlayerTroopsToPlace[player] += CalculateBonus(
            state.CardSetsTradedCount);
    }


    // Removes one card from player's hand
    private static void RemoveCard(ref GameState state, byte player, byte cardId)
    {
        ulong mask = 1UL << cardId;

        state.PlayerCardsBitboard[player] &= ~mask;
    }


    // Calculates reinforcement bonus
    private static byte CalculateBonus(byte tradedSets)
    {
        if (tradedSets == 1)
            return 4;

        if (tradedSets == 2)
            return 6;

        if (tradedSets == 3)
            return 8;

        return (byte)(10 + (tradedSets - 4) * 5);
    }
}