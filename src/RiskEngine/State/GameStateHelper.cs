using System.Numerics;
using System.Runtime.CompilerServices;

namespace RiskEngine.State;

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

    /// <summary>
    /// Setzt den Besitzer eines Territoriums und aktualisiert automatisch das inkrementelle Bitboard.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetTerritoryOwner(ref GameState state, int territoryId, byte owner)
    {
        byte oldOwner = state.TerritoryOwners[territoryId];

        // 1. Territorium beim alten Besitzer entfernen
        if (oldOwner != EngineConstants.NO_VALUE && oldOwner < EngineConstants.MAX_PLAYERS)
        {
            state.PlayerTerritoriesBitboard[oldOwner] &= ~(1UL << territoryId);
        }

        // 2. Neuer Besitzer setzen
        state.TerritoryOwners[territoryId] = owner;

        // 3. Territorium beim neuen Besitzer hinzufügen
        if (owner != EngineConstants.NO_VALUE && owner < EngineConstants.MAX_PLAYERS)
        {
            state.PlayerTerritoriesBitboard[owner] |= (1UL << territoryId);
        }
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
    /// Liefert das Bitboard aller vom Spieler besetzten Territorien in O(1).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetPlayerTerritoriesBitboard(in GameState state, byte player)
    {
        return state.PlayerTerritoriesBitboard[player];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetOwnedTerritoryCount(in GameState state, byte player)
    {
        return BitOperations.PopCount(state.PlayerTerritoriesBitboard[player]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetFirstTerritoryOwnedBy(in GameState state, byte player)
    {
        ulong territories = state.PlayerTerritoriesBitboard[player];

        if (territories == 0)
            return EngineConstants.NO_VALUE;

        return (byte)BitOperations.TrailingZeroCount(territories);
    }

    // ==========================================
    // --- PLAYER TROOPS & MISSIONS -------------
    // ==========================================

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetPlayerTroopsToPlace(in GameState state, byte player)
    {
        return state.PlayerTroopsToPlace[player];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetPlayerTroopsToPlace(ref GameState state, byte player, byte count)
    {
        state.PlayerTroopsToPlace[player] = count;
    }

    // ==========================================
    // --- PLAYER ALIVE -------------------------
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
    // --- WINNER & TERMINATION -----------------
    // ==========================================

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Terminate(ref GameState state, byte winnerId = EngineConstants.NO_VALUE)
    {
        state.WinnerId = winnerId;
        state.CurrentPhase = GamePhase.Terminated;
    }

    // ==========================================
    // --- FACTORY ------------------------------
    // ==========================================

    public static GameState CreateEmpty(byte playerCount = EngineConstants.MAX_PLAYERS, byte territoryCount = EngineConstants.MAX_TERRITORIES)
    {
        GameState state = default;

        state.CurrentRound = EngineConstants.NO_VALUE;
        state.CurrentPhase = GamePhase.Default;
        state.PlayersAliveBitboard = (byte)((1 << playerCount) - 1);
        state.WinnerId = EngineConstants.NO_VALUE;
        state.AttackerTerritory = EngineConstants.NO_VALUE;
        state.DefenderTerritory = EngineConstants.NO_VALUE;
        state.PlayerTurn = 0;
        state.CardSetsTradedCount = 0;

        for (int i = 0; i < playerCount; i++)
        {
            state.PlayerTerritoriesBitboard[i] = 0UL;
            state.PlayerCardsBitboard[i] = 0UL;
            state.PlayerTroopsToPlace[i] = 0;
            state.PlayerMissions[i] = EngineConstants.NO_VALUE;
        }

        for (int i = 0; i < territoryCount; i++)
        {
            state.TerritoryOwners[i] = EngineConstants.NO_VALUE;
            state.TerritoryTroops[i] = EngineConstants.NO_VALUE;
        }

        return state;
    }
}