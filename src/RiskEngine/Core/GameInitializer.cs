using RiskEngine.Mission;

namespace RiskEngine.State;

public static class GameInitializer
{
    public static unsafe GameState CreateInitialState(GameLayout layout, ref EngineRandom rng)
    {
        // 1. Get Params
        int territoryCount = layout.Map.TerritoryCount;
        var playerCount = layout.Config.PlayerCount;

        // 2. Reserve Memory Span for Territories
        Span<byte> territoryIds = stackalloc byte[territoryCount];
        for (byte i = 0; i < territoryCount; i++) territoryIds[i] = i;

        // 3. Shuffle Territories
        rng.Shuffle(territoryIds);

        // 4. Calculate Base Share and Remainder of Territories
        var baseTerritoriesPerPlayer = territoryCount / playerCount;
        var remainder = territoryCount % playerCount;

        // 5. Reserve Memory Span for Player Order
        Span<byte> playerOrder = stackalloc byte[playerCount];
        for (byte i = 0; i < playerCount; i++) playerOrder[i] = i;

        // 6. Shuffle Players (playerOrder)
        rng.Shuffle(playerOrder);

        // 7. Reserve Memory Span for Final Territory Owners Mapping
        Span<byte> territoryOwners = stackalloc byte[territoryCount];

        // 8. Territory Owner Mapping
        byte currentTerritoryIndex = 0;

        for (byte p = 0; p < playerCount; p++)
        {
            var playerId = playerOrder[p];

            // Every Player receives base share and the first remainder players receive +1
            var extra = (byte)(p < remainder ? 1 : 0);
            var countForThisPlayer = (byte)(baseTerritoriesPerPlayer + extra);

            for (byte k = 0; k < countForThisPlayer; k++)
            {
                var territoryId = territoryIds[currentTerritoryIndex++];
                territoryOwners[territoryId] = playerId;
            }
        }

        // 9. Initialize GameState
        var state = GameStateHelper.CreateEmpty(playerCount);

        // Assign owners and place exactly 1 initial troop per territory
        for (var i = 0; i < territoryCount; i++)
        {
            state.TerritoryOwners[i] = territoryOwners[i];
            state.TerritoryTroops[i] = 1;
        }

        // Set starting player (first player in shuffled order)
        state.PlayerTurn = playerOrder[0];

        // Reset troops to place for all players
        for (byte p = 0; p < playerCount; p++) state.PlayerTroopsToPlace[p] = 0;
        
        
        // 10. Assign each player one unique mission with fallback safety
        int missionCount = layout.Missions.Count;

        if (missionCount >= playerCount)
        {
            Span<byte> missionIds = stackalloc byte[missionCount];
            for (byte i = 0; i < missionCount; i++) missionIds[i] = i;

            // Shuffle Mission Catalog deterministically
            rng.Shuffle(missionIds);

            for (byte p = 0; p < playerCount; p++)
            {
                byte missionId = missionIds[p];
                var mission = layout.Missions[missionId];

                // CHECK: Self-Elimination OR Non-Existent Player ID
                if (mission.Type == MissionType.EliminatePlayer)
                {
                    bool isSelfElimination = mission.TargetPlayerId == p;
                    bool isInvalidPlayerId = mission.TargetPlayerId >= playerCount;

                    if (isSelfElimination || isInvalidPlayerId)
                    {
                        // Fallback: Convert to World Domination
                        state.PlayerMissions[p] = layout.Missions.FallbackMissionId;
                        continue;
                    }
                }

                // Valid mission assigned
                state.PlayerMissions[p] = missionId;
            }
        }
        else
        {
            // General Fallback if catalog is too small
            for (byte p = 0; p < playerCount; p++)
            {
                state.PlayerMissions[p] = layout.Missions.FallbackMissionId;
            }
        }
        
      

        return state;
    }
}