using System.Numerics;
using System.Runtime.CompilerServices;

namespace RiskEngine.State;

public static unsafe class CardHelper
{
    
    // ==========================================
    // --- CARDS -------------------------------
    // ==========================================

    /// <summary>
    /// Returns every card that is currently still in the deck.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetAvailableCards(in GameState state, DeckLayout deck)
    {
        ulong allCards = deck.AllCardsMask;

        ulong ownedCards = 0;

        for (byte player = 0; player < EngineConstants.MAX_PLAYERS; player++)
        {
            ownedCards |= state.PlayerCardsBitboard[player];
        }

        return allCards & ~ownedCards;
    }

    /// <summary>
    /// Draws one random remaining card.
    /// </summary>
    public static void GiveBonusCard(ref GameState state, byte player, ref EngineRandom rng, DeckLayout deck)
    {
        ulong available = GetAvailableCards(in state, deck);

        if (available == 0)
            return;

        int cardCount = BitOperations.PopCount(available);
        int index = rng.Next(0, cardCount);

        for (int i = 0; i < index; i++)
        {
            available &= available - 1;
        }

        int cardId = BitOperations.TrailingZeroCount(available);

        state.PlayerCardsBitboard[player] |= 1UL << cardId;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EliminateAndTransferCards(
        ref GameState state,
        byte attacker,
        byte defender)
    {
        state.PlayerCardsBitboard[attacker] |= state.PlayerCardsBitboard[defender];
        state.PlayerCardsBitboard[defender] = 0;
        GameStateHelper.EliminatePlayer(ref state, defender);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static  ulong GetPlayerCardsBitboard(
        in GameState state,
        byte player)
    {
        return state.PlayerCardsBitboard[player];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PlayerHasCard(
        in GameState state,
        byte player,
        byte cardId,
        DeckLayout deck)
    {
        if (cardId >= deck.CardCount)
            return false;

        return (state.PlayerCardsBitboard[player] & (1UL << cardId)) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddCardToPlayer(
        ref GameState state,
        byte player,
        byte cardId,
        DeckLayout deck)
    {
        if (cardId >= deck.CardCount)
            return;

        state.PlayerCardsBitboard[player] |= 1UL << cardId;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveCardFromPlayer(
        ref GameState state,
        byte player,
        byte cardId,
        DeckLayout deck)
    {
        if (cardId >= deck.CardCount)
            return;

        state.PlayerCardsBitboard[player] &= ~(1UL << cardId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetPlayerCardCount(
        in GameState state,
        byte player)
    {
        return BitOperations.PopCount(state.PlayerCardsBitboard[player]);
    }

    public static int ExtractCardIds(ulong bitboard, Span<byte> buffer)
    {
        int count = 0;

        while (bitboard != 0)
        {
            int id = BitOperations.TrailingZeroCount(bitboard);

            buffer[count++] = (byte)id;

            bitboard &= bitboard - 1;
        }

        return count;
    }
    
    
    /// <summary>
    ///     Checks if a player holds at least one valid card set that can be turned in.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasValidSet(in GameState state, byte player, DeckLayout deckLayout)
    {
        var hand = GetPlayerCardsBitboard(in state, player);

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
    
}