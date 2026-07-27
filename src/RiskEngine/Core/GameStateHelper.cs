using System.Numerics;
using System.Runtime.CompilerServices;

namespace RiskEngine;

public static unsafe class GameStateHelper
{
    // ==========================================
    // --- TERRITORY GETTERS & SETTERS ----------
    // ==========================================

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetTerritoryOwner(in GameState state, int territoryId)
    {
        return state.TerritoryOwners[territoryId];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetTerritoryOwner(ref GameState state, int territoryId, byte owner)
    {
        state.TerritoryOwners[territoryId] = owner;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetTerritoryTroops(in GameState state, int territoryId)
    {
        return state.TerritoryTroops[territoryId];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetTerritoryTroops(ref GameState state, int territoryId, byte count)
    {
        state.TerritoryTroops[territoryId] = count;
    }

    /// <summary>
    ///     Constructs a bitboard representation of all territories currently owned by the specified player.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetPlayerTerritoriesBitboard(in GameState state, byte player)
    {
        var mask = 0UL;

        for (var i = 0; i < EngineConstants.DEFAULT_TERRITORY_COUNT; i++)
            if (state.TerritoryOwners[i] == player)
                mask |= 1UL << i;

        return mask;
    }

    /// <summary>
    ///     Returns the total number of territories currently owned by the specified player.
    ///     Uses hardware PopCount for O(1) performance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetOwnedTerritoryCount(in GameState state, byte player)
    {
        var playerTerritoriesBitboard = GetPlayerTerritoriesBitboard(in state, player);
        return BitOperations.PopCount(playerTerritoriesBitboard);
    }

    /// <summary>
    /// Finds the first territory ID owned by the specified player.
    /// Returns NO_VALUE if the player owns no territories.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetFirstTerritoryOwnedBy(in GameState state, byte player)
    {
        var playerTerritoriesBitboard = GetPlayerTerritoriesBitboard(in state, player);

        if (playerTerritoriesBitboard == 0UL)
            return EngineConstants.NO_VALUE;

        return (byte)BitOperations.TrailingZeroCount(playerTerritoriesBitboard);
    }

    /// <summary>
    ///     Gets the number of unplaced reinforcement troops available for the specified player.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetPlayerTroopsToPlace(in GameState state, byte player)
    {
        return state.PlayerTroopsToPlace[player];
    }

    /// <summary>
    ///     Sets the number of unplaced reinforcement troops available for the specified player.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetPlayerTroopsToPlace(ref GameState state, byte player, byte count)
    {
        state.PlayerTroopsToPlace[player] = count;
    }

    /// <summary>
    ///     Checks if the player owns at least one territory with > 1 troops
    ///     that is adjacent to an enemy territory.
    /// </summary>
    public static bool CanPlayerAttack(in GameState state, byte player, MapLayout map)
    {
        for (var i = 0; i < EngineConstants.DEFAULT_TERRITORY_COUNT; i++)
        {
            // 1. Territorium muss dem Spieler gehören
            if (state.TerritoryOwners[i] != player) continue;

            // 2. Muss mehr als 1 Truppe haben (mind. 1 muss stehen bleiben)
            if (state.TerritoryTroops[i] <= 1) continue;

            // 3. Prüfen, ob mind. ein Nachbar ein Feind ist
            var neighbors = map.Adjacencies[i];
            for (var n = 0; n < neighbors.Length; n++)
            {
                var neighborId = neighbors[n];
                if (state.TerritoryOwners[neighborId] !=
                    player) return true; // Mindestens 1 valider Angriff ist theoretisch noch möglich!
            }
        }

        return false; // Keine Angriffsmöglichkeiten mehr vorhanden
    }

    // ==========================================
    // --- PLAYER ALIVE BITBOARD ----------------
    // ==========================================

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPlayerAlive(in GameState state, byte player)
    {
        return (state.PlayersAliveBitboard & (1 << player)) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetPlayerAlive(ref GameState state, byte player)
    {
        state.PlayersAliveBitboard |= (byte)(1 << player);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EliminatePlayer(ref GameState state, byte player)
    {
        state.PlayersAliveBitboard &= (byte)~(1 << player);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetActivePlayerCount(in GameState state)
    {
        return BitOperations.PopCount(state.PlayersAliveBitboard);
    }

    // ==========================================
    // --- CARD BITBOARD HELPERS ----------------
    // ==========================================

    /// <summary>
    /// Returns all cards currently available in the deck.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetAvailableCards(in GameState state)
    {
        ulong allCards = (1UL << EngineConstants.DEFAULT_TERRITORY_COUNT) - 1;

        ulong ownedCards = 0;

        for (byte player = 0; player < EngineConstants.MAX_PLAYERS; player++)
        {
            ownedCards |= state.PlayerCardsBitboard[player];
        }

        return allCards & ~ownedCards;
    }
    
    /// <summary>
    /// Draws a random available card and gives it to the player.
    /// </summary>
    public static void GiveBonusCard(ref GameState state, byte player, ref EngineRandom rng)
    {
        ulong deck = GetAvailableCards(in state);

        // No cards available
        if (deck == 0)
            return;

        int remainingCards = BitOperations.PopCount(deck);
        int targetIndex = rng.Next(0,remainingCards);

        // Find selected card
        for (int i = 0; i < targetIndex; i++)
        {
            deck &= deck - 1;
        }

        int cardId = BitOperations.TrailingZeroCount(deck);
        ulong mask = 1UL << cardId;

        // Give card to player
        state.PlayerCardsBitboard[player] |= mask;
    }
    
    /// <summary>
    ///     Transfers all cards from defender to attacker and marks the defender as eliminated in O(1).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EliminateAndTransferCards(ref GameState state, byte attacker, byte defender)
    {
        // 1. All cards from defender to attacker (Bitwise OR)
        state.PlayerCardsBitboard[attacker] |= state.PlayerCardsBitboard[defender];

        // 2. Clear defender's hand
        state.PlayerCardsBitboard[defender] = 0UL;

        // 3. Mark defender as eliminated in bitboard
        EliminatePlayer(ref state, defender);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetPlayerCardsBitboard(in GameState state, byte player)
    {
        return state.PlayerCardsBitboard[player];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PlayerHasCard(in GameState state, byte player, byte cardId)
    {
        if (cardId >= EngineConstants.DEFAULT_TERRITORY_COUNT) return false;
        return (state.PlayerCardsBitboard[player] & (1UL << cardId)) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddCardToPlayer(ref GameState state, byte player, byte cardId)
    {
        if (cardId < EngineConstants.DEFAULT_TERRITORY_COUNT) state.PlayerCardsBitboard[player] |= 1UL << cardId;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveCardFromPlayer(ref GameState state, byte player, byte cardId)
    {
        if (cardId < EngineConstants.DEFAULT_TERRITORY_COUNT) state.PlayerCardsBitboard[player] &= ~(1UL << cardId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetPlayerCardCount(in GameState state, byte player)
    {
        return BitOperations.PopCount(state.PlayerCardsBitboard[player]);
    }

    // ==========================================
    // --- FACTORY / INITIALIZER ---------------
    // ==========================================

    public static GameState CreateEmpty(byte playerCount = EngineConstants.MAX_PLAYERS)
    {
        GameState state = default;

        state.CurrentRound = 1;
        state.CurrentPhase = GamePhase.Default;

        state.PlayersAliveBitboard = (byte)((1 << playerCount) - 1);

        for (var i = 0; i < EngineConstants.DEFAULT_TERRITORY_COUNT; i++)
        {
            state.TerritoryOwners[i] = EngineConstants.NO_VALUE;
            state.TerritoryTroops[i] = 0;
        }

        return state;
    }
}