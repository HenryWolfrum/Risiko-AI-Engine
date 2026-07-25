namespace RiskEngine;

public unsafe struct GameState
{
    // --- Territory Info ---
    public fixed byte TerritoryOwners[EngineConstants.DEFAULT_TERRITORY_COUNT];
    public fixed byte TerritoryTroops[EngineConstants.DEFAULT_TERRITORY_COUNT];

    // --- Player Info ---
    public fixed byte PlayerTroopsToPlace[EngineConstants.MAX_PLAYERS];
    public fixed bool IsPlayerAlive[EngineConstants.MAX_PLAYERS];

    // --- Cards & Deck Info ---
    public ulong CardDeckBitboard;
    public fixed ulong PlayerCardsBitboard[EngineConstants.MAX_PLAYERS];
    public byte CardSetsTradedCount;

    // --- History / Game Loop ---
    public ushort CurrentRound;
    public byte PlayerTurn;
    public GamePhase CurrentPhase;

    // --- Phase: Conquer ---
    public bool HasConqueredTerritoryThisTurn;

    // --- Phase: Attack ---
    public byte SelectedAttackerTerritory;
    public byte SelectedDefenderTerritory;

    // --- Phase: Fortify / Move ---
    public byte SelectedFortifySource;
    public byte SelectedFortifyTarget;

    // --- Dice Info ---
    public byte LastAttackerDiceCount;
    public byte LastDefenderDiceCount;
    public fixed byte LastAttackerDiceValues[EngineConstants.ATTACKER_DICE_COUNT];
    public fixed byte LastDefenderDiceValues[EngineConstants.DEFENDER_DICE_COUNT];

    
    /// <summary>Gets the owner of a territory safely.</summary>
    public readonly byte GetTerritoryOwner(int index)
    {
        return TerritoryOwners[index];
    }

    /// <summary>Gets the troop count of a territory safely.</summary>
    public readonly byte GetTerritoryTroops(int index)
    {
        return TerritoryTroops[index];
    }
    
    /// <summary>Gets the troop count player can use.</summary>
    public readonly byte GetPlayerTroopsToPlace(byte player)
    {
        return PlayerTroopsToPlace[player];
    }
    
    
    /// <summary>Sets the troop count player can use.</summary>

    public void SetPlayerTroopsToPlace(byte player, byte amount)
    {
        PlayerTroopsToPlace[player] = amount;
    }
   
    /// <summary>
    /// Creates a clean, zeroed GameState instance on the stack.
    /// </summary>
    public static GameState CreateEmpty()
    {
        GameState state = default;

        state.SelectedAttackerTerritory = EngineConstants.NO_VALUE;
        state.SelectedDefenderTerritory = EngineConstants.NO_VALUE;

        state.SelectedFortifySource = EngineConstants.NO_VALUE;
        state.SelectedFortifyTarget = EngineConstants.NO_VALUE;

        state.LastAttackerDiceCount = EngineConstants.NO_VALUE;
        state.LastDefenderDiceCount = EngineConstants.NO_VALUE;

        state.CurrentRound = 1;
        state.CurrentPhase = GamePhase.Default;

        for (int i = 0; i < EngineConstants.MAX_PLAYERS; i++)
        {
            state.IsPlayerAlive[i] = true;
        }

        return state;
    }
}