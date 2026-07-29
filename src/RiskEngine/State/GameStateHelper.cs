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
    /// Creates a bitboard containing every territory owned by the player.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetPlayerTerritoriesBitboard(in GameState state, byte player)
    {
        ulong mask = 0;

        for (int i = 0; i < EngineConstants.MAX_TERRITORIES; i++)
        {
            if (state.TerritoryOwners[i] == player)
            {
                mask |= 1UL << i;
            }
        }

        return mask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetOwnedTerritoryCount(in GameState state, byte player)
    {
        return BitOperations.PopCount(GetPlayerTerritoriesBitboard(in state, player));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetFirstTerritoryOwnedBy(in GameState state, byte player)
    {
        ulong territories = GetPlayerTerritoriesBitboard(in state, player);

        if (territories == 0)
            return EngineConstants.NO_VALUE;

        return (byte)BitOperations.TrailingZeroCount(territories);
    }

    // ==========================================
    // --- PLAYER TROOPS ------------------------
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
    // --- WINNER ------------------------------
    // ==========================================
    
    
    /// <summary>
    /// Safely terminates the game and registers the winning player.
    /// Pass EngineConstants.NO_VALUE (255) if the game ended without a winner (e.g. max rounds reached).
    /// </summary>
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

        state.CurrentRound = 1;
        state.CurrentPhase = GamePhase.Default;
        state.PlayersAliveBitboard = (byte)((1 << playerCount) - 1);

        for (int i = 0; i < territoryCount; i++)
        {
            state.TerritoryOwners[i] = EngineConstants.NO_VALUE;
            state.TerritoryTroops[i] = 0;
        }

        return state;
    }
}