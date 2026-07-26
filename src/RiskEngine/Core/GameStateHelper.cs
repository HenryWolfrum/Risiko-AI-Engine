namespace RiskEngine;

using System.Numerics;
using System.Runtime.CompilerServices;

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
    /// Constructs a bitboard representation of all territories currently owned by the specified player.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetPlayerTerritoriesBitboard(in GameState state, byte player)
    {
        ulong mask = 0UL;

        for (int i = 0; i < EngineConstants.DEFAULT_TERRITORY_COUNT; i++)
        {
            if (state.TerritoryOwners[i] == player)
            {
                mask |= (1UL << i);
            }
        }

        return mask;
    }
    
    /// <summary>
    /// Returns the total number of territories currently owned by the specified player.
    /// Uses hardware PopCount for O(1) performance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetOwnedTerritoryCount(in GameState state, byte player)
    {
        ulong playerTerritoriesBitboard = GetPlayerTerritoriesBitboard(in state, player);
        return BitOperations.PopCount(playerTerritoriesBitboard);
    }

    /// <summary>
    /// Finds the territory ID (0-indexed) of the first territory owned by the specified player.
    /// Uses hardware TrailingZeroCount (TZCNT) for O(1) performance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetFirstTerritoryOwnedBy(in GameState state, byte player)
    {
        ulong playerTerritoriesBitboard = GetPlayerTerritoriesBitboard(in state, player);
        
        // Safety check: returns 0 if player owns no territories (e.g. eliminated)
        if (playerTerritoriesBitboard == 0UL)
            return 0;

        return (byte)BitOperations.TrailingZeroCount(playerTerritoriesBitboard);
    }
    
    /// <summary>
    /// Gets the number of unplaced reinforcement troops available for the specified player.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetPlayerTroopsToPlace(in GameState state, byte player)
    {
        return state.PlayerTroopsToPlace[player];
    }

    /// <summary>
    /// Sets the number of unplaced reinforcement troops available for the specified player.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetPlayerTroopsToPlace(ref GameState state, byte player, byte count)
    {
        state.PlayerTroopsToPlace[player] = count;
    }
    
    /// <summary>
    /// Checks if the player owns at least one territory with > 1 troops 
    /// that is adjacent to an enemy territory.
    /// </summary>
    public static bool CanPlayerAttack(in GameState state, byte player, MapLayout map)
    {
        for (int i = 0; i < EngineConstants.DEFAULT_TERRITORY_COUNT; i++)
        {
            // 1. Territorium muss dem Spieler gehören
            if (state.TerritoryOwners[i] != player) continue;

            // 2. Muss mehr als 1 Truppe haben (mind. 1 muss stehen bleiben)
            if (state.TerritoryTroops[i] <= 1) continue;

            // 3. Prüfen, ob mind. ein Nachbar ein Feind ist
            byte[] neighbors = map.Adjacencies[i];
            for (int n = 0; n < neighbors.Length; n++)
            {
                byte neighborId = neighbors[n];
                if (state.TerritoryOwners[neighborId] != player)
                {
                    return true; // Mindestens 1 valider Angriff ist theoretisch noch möglich!
                }
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
    /// Transfers all cards from defender to attacker and marks the defender as eliminated in O(1).
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
        if (cardId < EngineConstants.DEFAULT_TERRITORY_COUNT)
        {
            state.PlayerCardsBitboard[player] |= (1UL << cardId);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveCardFromPlayer(ref GameState state, byte player, byte cardId)
    {
        if (cardId < EngineConstants.DEFAULT_TERRITORY_COUNT)
        {
            state.PlayerCardsBitboard[player] &= ~(1UL << cardId);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetPlayerCardCount(in GameState state, byte player)
    {
      
        return System.Numerics.BitOperations.PopCount(state.PlayerCardsBitboard[player]);
    }

    // ==========================================
    // --- FACTORY / INITIALIZER ---------------
    // ==========================================

    public static GameState CreateEmpty(byte playerCount = EngineConstants.MAX_PLAYERS)
    {
        GameState state = default;

        state.CurrentRound = 1;
        state.CurrentPhase = GamePhase.Default;

        // Set bit to 1 for all active players
        state.PlayersAliveBitboard = (byte)((1 << playerCount) - 1);

        return state;
    }
}