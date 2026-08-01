using RiskEngine.Mission;

namespace RiskEngine.Mission;

using RiskEngine.State;

public static class MissionEvaluator
{
    /// <summary>
    /// Checks whether the active player has fulfilled their assigned mission.
    /// </summary>
    public static bool IsFulfilled(in GameState state, in GameLayout layout, byte playerIndex)
    {
        byte missionId = MissionHelper.GetPlayerMission(in state, playerIndex);
        ref readonly var mission = ref layout.Missions[missionId];

        bool isFulfilled = mission.Type switch
        {
            MissionType.WorldDomination => CheckWorldDomination(in state, in layout, playerIndex),
            MissionType.ConquerTerritories => CheckTerritories(in state, in layout, playerIndex, in mission),
            MissionType.ConquerContinents => CheckContinents(in state, in layout, playerIndex, in mission),
            MissionType.EliminatePlayer => CheckElimination(in state, in layout, playerIndex, in mission),
            _ => false
        };

    
        return isFulfilled;
    }

    /// <summary>
    /// Triggered ONLY when a player territory count drops to 0 during the conquer phase (Event-Driven).
    /// Checks if ANY player (active or passive) held an elimination mission targeting the victim.
    /// </summary>
    public static bool CheckEliminationWin(in GameState state, in GameLayout layout, byte eliminatedPlayerId, out byte winnerId)
    {
        for (byte i = 0; i < layout.Config.PlayerCount; i++)
        {
            if (!GameStateHelper.IsPlayerAlive(in state, i))
                continue;

            byte missionId = MissionHelper.GetPlayerMission(in state, i);
            ref readonly var mission = ref layout.Missions[missionId];

            if (mission.Type == MissionType.EliminatePlayer && mission.TargetPlayerId == eliminatedPlayerId)
            {
                winnerId = i;
                return true;
            }
        }

        winnerId = 0;
        return false;
    }

    private static bool CheckWorldDomination(in GameState state, in GameLayout layout, byte player)
        => GameStateHelper.GetOwnedTerritoryCount(in state, player) == layout.Map.TerritoryCount;

    private static bool CheckTerritories(in GameState state, in GameLayout layout, byte player, in MissionDefinition mission)
    {
        int count = GameStateHelper.GetOwnedTerritoryCount(in state, player);
        if (count < mission.RequiredTerritories) 
            return false;

        if (mission.MinTroopsPerTerritory <= 1) 
            return true;

        return MissionHelper.HasMinTroopsOnAllTerritories(in state,in layout, player, mission.MinTroopsPerTerritory);
    }

    private static bool CheckContinents(in GameState state, in GameLayout layout, byte player, in MissionDefinition mission)
    {
        byte playerContinents = MissionHelper.GetControlledContinentMask(in state, layout, player);

        // Verify that all mandatory continents defined in the target mask are controlled
        if ((playerContinents & mission.TargetContinentMask) != mission.TargetContinentMask)
        {
            return false;
        }


        return true;
    }

    private static bool CheckElimination(in GameState state, in GameLayout layout, byte player,
        in MissionDefinition mission)
    {
        // Is target still alive
        return !GameStateHelper.IsPlayerAlive(in state, mission.TargetPlayerId);

    }
}