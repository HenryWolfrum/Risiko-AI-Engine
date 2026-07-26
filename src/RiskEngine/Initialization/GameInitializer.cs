namespace RiskEngine;

public static class GameInitializer
{
    public static unsafe GameState CreateInitialState(GameLayout layout, int seed)
    {
        // 0. Deterministic Random Generator
        EngineRandom rng = new EngineRandom(seed);

        // 1. Get Params
        int territoryCount = layout.Config.TerritoryCount;
        byte playerCount = layout.Config.PlayerCount;

        // 2. Reserve Memory Span for Territories
        Span<byte> territoryIds = stackalloc byte[territoryCount];
        for (byte i = 0; i < territoryCount; i++)
        {
            territoryIds[i] = i;
        }

        // 3. Shuffle Territories
        rng.Shuffle(territoryIds);

        // 4. Calculate Base Share and Remainder of Territories
        int baseTerritoriesPerPlayer = territoryCount / playerCount;
        int remainder = territoryCount % playerCount;

        // 5. Reserve Memory Span for Player Order
        Span<byte> playerOrder = stackalloc byte[playerCount];
        for (byte i = 0; i < playerCount; i++)
        {
            playerOrder[i] = i;
        }
        
        // 6. Shuffle Players (playerOrder)
        rng.Shuffle(playerOrder);

        // 7. Reserve Memory Span for Final Territory Owners Mapping
        Span<byte> territoryOwners = stackalloc byte[territoryCount];

        // 8. Territory Owner Mapping
        byte currentTerritoryIndex = 0;

        for (byte p = 0; p < playerCount; p++)
        {
            byte playerId = playerOrder[p];

            // Every Player receives base share and the first remainder players receive +1
            byte extra = (byte)(p < remainder ? 1 : 0);
            byte countForThisPlayer = (byte)(baseTerritoriesPerPlayer + extra);

            for (byte k = 0; k < countForThisPlayer; k++)
            {
                byte territoryId = territoryIds[currentTerritoryIndex++];
                territoryOwners[territoryId] = playerId;
            }
        }

        // 9. Initialize GameState
        GameState state = GameStateHelper.CreateEmpty(playerCount);

        // Assign owners and place exactly 1 initial troop per territory
        for (int i = 0; i < territoryCount; i++)
        {
            state.TerritoryOwners[i] = territoryOwners[i];
            state.TerritoryTroops[i] = 1;
        }

        // Set starting player (first player in shuffled order)
        state.PlayerTurn = playerOrder[0];

        // Reset troops to place for all players
        for (byte p = 0; p < playerCount; p++)
        {
            state.PlayerTroopsToPlace[p] = 0;
        }

        return state;
    }
}