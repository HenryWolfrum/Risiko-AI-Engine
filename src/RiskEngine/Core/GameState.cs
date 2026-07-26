namespace RiskEngine;

public unsafe struct GameState
{
    // --- Territory Info ---
    public fixed byte TerritoryOwners[EngineConstants.DEFAULT_TERRITORY_COUNT];
    public fixed byte TerritoryTroops[EngineConstants.DEFAULT_TERRITORY_COUNT];

    // --- Player Info ---
    public fixed byte PlayerTroopsToPlace[EngineConstants.MAX_PLAYERS];

    // --- Cards & Deck Info ---
    public ulong CardDeckBitboard;
    public fixed ulong PlayerCardsBitboard[EngineConstants.MAX_PLAYERS];
    public byte CardSetsTradedCount;

    // --- History / Game Loop ---
    public ushort CurrentRound;
    public byte PlayerTurn;
    public GamePhase CurrentPhase;
    
    
    
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
    
    /// <summary>Sets the troop count of a territory.</summary>
    public void SetTerritoryTroops(byte territory, byte amount)
    {
        TerritoryTroops[territory] = amount;
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
   
    /// <summary>Checks if a player holds a specific card (territory ID 0-41) in their hand.</summary>
    public readonly bool PlayerHasCard(byte player, byte cardId)
    {
        if (cardId >= 42) return false;
        
        // Check if the bit at the card's position is set (1)
        return (PlayerCardsBitboard[player] & (1UL << cardId)) != 0;
    }

    /// <summary>Adds a card to a player's hand by setting its corresponding bit.</summary>
    public void AddCardToPlayer(byte player, byte cardId)
    {
        if (cardId < 42)
        {
            // Set the bit for the specified card ID
            PlayerCardsBitboard[player] |= (1UL << cardId);
        }
    }

    /// <summary>Removes a card from a player's hand by clearing its corresponding bit.</summary>
    public void RemoveCardFromPlayer(byte player, byte cardId)
    {
        if (cardId < 42)
        {
            // Clear the bit for the specified card ID using a bitwise NOT mask
            PlayerCardsBitboard[player] &= ~(1UL << cardId);
        }
    }
    
    
    
    /// <summary>
    /// Creates a clean, zeroed GameState instance on the stack.
    /// </summary>
    public static GameState CreateEmpty()
    {
        GameState state = default;
        
        state.CurrentRound = 1;
        state.CurrentPhase = GamePhase.Default;
        
        return state;
    }
}