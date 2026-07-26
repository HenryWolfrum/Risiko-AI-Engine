using System.Numerics;
using System.Runtime.CompilerServices;

namespace RiskEngine;

public static class CardHelper
{
    /// <summary>
    ///     Checks if a player holds at least one valid card set that can be turned in.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasValidSet(in GameState state, byte player, DeckLayout deckLayout)
    {
        var hand = GameStateHelper.GetPlayerCardsBitboard(in state, player);

        // Early exit 1: Impossible to form a set with less than 3 cards
        if (BitOperations.PopCount(hand) < 3)
            return false;

        // Early exit 2: Holding 3+ cards including at least one Joker guarantees a valid set
        if ((hand & deckLayout.JokerMask) != 0)
            return true;

        // Count total cards for each specific type using hardware PopCount
        var infantryCount = BitOperations.PopCount(hand & deckLayout.InfantryMask);
        var cavalryCount = BitOperations.PopCount(hand & deckLayout.CavalryMask);
        var artilleryCount = BitOperations.PopCount(hand & deckLayout.ArtilleryMask);

        // Condition A: Three of the same type
        if (infantryCount >= 3 || cavalryCount >= 3 || artilleryCount >= 3)
            return true;

        // Condition B: One card of each distinct type (1x Infantry + 1x Cavalry + 1x Artillery)
        return infantryCount > 0 && cavalryCount > 0 && artilleryCount > 0;
    }

    /// <summary>
    ///     Finds the first valid card set for a player and constructs a GameAction.
    ///     Note: A valid set is mathematically guaranteed if player holds >= 5 cards.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameAction FindFirstValidSet(in GameState state, byte player, DeckLayout deckLayout)
    {
        var hand = GameStateHelper.GetPlayerCardsBitboard(in state, player);

        var infantry = hand & deckLayout.InfantryMask;
        var cavalry = hand & deckLayout.CavalryMask;
        var artillery = hand & deckLayout.ArtilleryMask;

        // --- Priority 1: Three of the same card type ---
        if (BitOperations.PopCount(infantry) >= 3)
            return CreateTurnInAction(infantry);

        if (BitOperations.PopCount(cavalry) >= 3)
            return CreateTurnInAction(cavalry);

        if (BitOperations.PopCount(artillery) >= 3)
            return CreateTurnInAction(artillery);

        // --- Priority 2: One card of each distinct type ---
        if (infantry != 0 && cavalry != 0 && artillery != 0)
            return new GameAction
            {
                Type = ActionType.TurnInCards,
                Card1 = (byte)BitOperations.TrailingZeroCount(infantry),
                Card2 = (byte)BitOperations.TrailingZeroCount(cavalry),
                Card3 = (byte)BitOperations.TrailingZeroCount(artillery)
            };

        // --- Priority 3: Joker combinations (1 Joker + any 2 other cards) ---
        var jokers = hand & deckLayout.JokerMask;
        if (jokers != 0)
        {
            var jokerId = (byte)BitOperations.TrailingZeroCount(jokers);
            var nonJokers = hand & ~deckLayout.JokerMask;

            // Extract the first two non-joker cards
            var c1 = (byte)BitOperations.TrailingZeroCount(nonJokers);
            nonJokers &= nonJokers - 1; // Clear lowest set bit
            var c2 = (byte)BitOperations.TrailingZeroCount(nonJokers);

            return new GameAction
            {
                Type = ActionType.TurnInCards,
                Card1 = jokerId,
                Card2 = c1,
                Card3 = c2
            };
        }

        return default;
    }

    /// <summary>
    ///     Extracts the first 3 card IDs from a bitmask and builds a TurnInCards GameAction.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static GameAction CreateTurnInAction(ulong mask)
    {
        var c1 = (byte)BitOperations.TrailingZeroCount(mask);
        mask &= mask - 1; // Clear lowest set bit using BMI1 / x & (x - 1)

        var c2 = (byte)BitOperations.TrailingZeroCount(mask);
        mask &= mask - 1;

        var c3 = (byte)BitOperations.TrailingZeroCount(mask);

        return new GameAction
        {
            Type = ActionType.TurnInCards,
            Card1 = c1,
            Card2 = c2,
            Card3 = c3
        };
    }
}