using System.Runtime.CompilerServices;

namespace RiskEngine.Mutation;

public static class CardTurnInMutator
{
    // Trades three cards for reinforcement troops
    public static void Apply(ref GameState state, in GameAction action)
    {
        var player = state.PlayerTurn;

        // 1. Remove cards from player's hand (fixed typo: Card3 instead of duplicate Card1)
        GameStateHelper.RemoveCardFromPlayer(ref state, player, action.Card1);
        GameStateHelper.RemoveCardFromPlayer(ref state, player, action.Card2);
        GameStateHelper.RemoveCardFromPlayer(ref state, player, action.Card3);

        // 2. Increase traded set counter
        state.CardSetsTradedCount++;

        // 3. Add base reinforcement bonus from card trade to player's placement pool
        var baseBonus = CalculateBonus(state.CardSetsTradedCount);
        var currentTroopsToPlace = GameStateHelper.GetPlayerTroopsToPlace(in state, player);
        GameStateHelper.SetPlayerTroopsToPlace(ref state, player, (byte)(currentTroopsToPlace + baseBonus));

        // 4. Check for territory ownership bonus (+2 troops directly onto the owned territory)
        ApplyTerritoryCardBonusIfOwned(ref state, player, action.Card1);
        ApplyTerritoryCardBonusIfOwned(ref state, player, action.Card2);
        ApplyTerritoryCardBonusIfOwned(ref state, player, action.Card3);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ApplyTerritoryCardBonusIfOwned(ref GameState state, byte player, byte cardTerritoryId)
    {
        // If the card matches a territory currently owned by the player, add +2 troops directly to that territory
        if (GameStateHelper.GetTerritoryOwner(in state, cardTerritoryId) == player)
        {
            var currentTroops = GameStateHelper.GetTerritoryTroops(in state, cardTerritoryId);
            GameStateHelper.SetTerritoryTroops(ref state, cardTerritoryId,
                (byte)(currentTroops + EngineConstants.CARD_TERRITORY_BONUS_TROOPS));
        }
    }

    // Calculates reinforcement bonus based on set count
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte CalculateBonus(byte tradedSets)
    {
        return tradedSets switch
        {
            1 => 4,
            2 => 6,
            3 => 8,
            4 => 10,
            5 => 12,
            6 => 15,
            _ => (byte)(15 + (tradedSets - 6) * 5)
        };
    }
}