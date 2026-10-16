using System.Numerics;
using System.Runtime.CompilerServices;

namespace RiskEngine.Layout;

/// <summary>
/// Provides helper methods for managing player cards using bitboards,
/// including drawing, transferring, and validating card sets.
/// </summary>
public static unsafe class CardHelper
{
    /// <summary>
    /// Calculates the remaining cards in the draw pile
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

        return allCards & ~ownedCards & ~state.DiscardCardsBitboard;
    }
    
    /// <summary>
    /// Draws a random card for the player, recycling the discard pile if necessary.
    /// </summary>
    public static void GiveBonusCard(ref GameState state, byte player, ref EngineRandom rng, DeckLayout deck)
    {
        ulong available = GetAvailableCards(in state, deck);

        
        if (available == 0)
        {
            state.DiscardCardsBitboard = 0UL;
            
            available = GetAvailableCards(in state, deck);
            
            if (available == 0)
                return;
        }

        int cardCount = BitOperations.PopCount(available);
        int index = rng.Next(0, cardCount);

        for (int i = 0; i < index; i++)
        {
            available &= available - 1;
        }

        int cardId = BitOperations.TrailingZeroCount(available);

        state.PlayerCardsBitboard[player] |= 1UL << cardId;
    }

    /// <summary>
    /// Transfers the defender's cards to the attacker and eliminates the defender.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EliminateAndTransferCards(ref GameState state, byte attacker, byte defender)
    {
        state.PlayerCardsBitboard[attacker] |= state.PlayerCardsBitboard[defender];
        state.PlayerCardsBitboard[defender] = 0;
        GameStateHelper.EliminatePlayer(ref state, defender);
    }

   
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetPlayerCardsBitboard(in GameState state, byte player)
    {
        return state.PlayerCardsBitboard[player];
    }

    /// <summary>
    /// Checks whether player owns the card (by Id)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PlayerHasCard(in GameState state, byte player, byte cardId, DeckLayout deck)
    {
        if (cardId >= deck.CardCount)
            return false;

        return (state.PlayerCardsBitboard[player] & (1UL << cardId)) != 0;
    }

    /// <summary>
    /// Adds a card to the player's hand and removes it from the discard pile.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddCardToPlayer(ref GameState state, byte player, byte cardId, DeckLayout deck)
    {
        if (cardId >= deck.CardCount)
            return;

        ulong mask = 1UL << cardId;
        state.PlayerCardsBitboard[player] |= mask;
        state.DiscardCardsBitboard &= ~mask; 
    }

    /// <summary>
    /// Removes a card from the player's hand and adds it to the discard pile.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveCardFromPlayer(ref GameState state, byte player, byte cardId, DeckLayout deck)
    {
        if (cardId >= deck.CardCount)
            return;

        ulong mask = 1UL << cardId;
        state.PlayerCardsBitboard[player] &= ~mask;
        state.DiscardCardsBitboard |= mask;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetPlayerCardCount(in GameState state, byte player)
    {
        return BitOperations.PopCount(state.PlayerCardsBitboard[player]);
    }

    /// <summary>
    /// Extracts the IDs of all cards represented by set bits in the bitboard.
    /// </summary>
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
    /// Checks if a player holds at least one valid card set that can be turned in.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasValidSet(in GameState state, byte player, DeckLayout deckLayout)
    {
        var hand = GetPlayerCardsBitboard(in state, player);

        if (BitOperations.PopCount(hand) < 3)
            return false;

        if ((hand & deckLayout.JokerMask) != 0)
            return true;

        var infantryCount = BitOperations.PopCount(hand & deckLayout.InfantryMask);
        var cavalryCount = BitOperations.PopCount(hand & deckLayout.CavalryMask);
        var artilleryCount = BitOperations.PopCount(hand & deckLayout.ArtilleryMask);

        if (infantryCount >= 3 || cavalryCount >= 3 || artilleryCount >= 3)
            return true;

        return infantryCount > 0 && cavalryCount > 0 && artilleryCount > 0;
    }
}