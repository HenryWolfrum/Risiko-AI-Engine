namespace RiskEngine.Mutation;

public static  class CardTurnInMutator
{
    // Trades three cards for reinforcement troops
    public static void Apply(ref GameState state, in GameAction action)
    {
        byte player = state.PlayerTurn;

        // Remove cards from player's hand
        state.RemoveCardFromPlayer(player,action.Card1);
        state.RemoveCardFromPlayer(player,action.Card2);
        state.RemoveCardFromPlayer(player,action.Card3);

        // Increase traded set counter
        state.CardSetsTradedCount++;

        // Add reinforcement bonus
        state.SetPlayerTroopsToPlace(player,(byte)(state.GetPlayerTroopsToPlace(player) + CalculateBonus(state.CardSetsTradedCount)));
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