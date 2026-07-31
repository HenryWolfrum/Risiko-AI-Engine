using System.Runtime.CompilerServices;

namespace RiskEngine.State;

public static class ReinforcementCalculator
{
    /// <summary>
    ///     Calculates base reinforcements + continent bonuses for the active player at turn start.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte CalculateTroops(in GameState state, MapLayout map)
    {
        var player = state.PlayerTurn;

        // 1. Base income: floor(owned / 3) with a minimum threshold of usually 3
        var ownedCount = GameStateHelper.GetOwnedTerritoryCount(in state, player);
        var totalTroops = Math.Max(EngineConstants.MIN_REINFORCEMENT_TROOPS, ownedCount / 3);

        // 2. Continent bonus check via precalculated Bitboards
        var playerTerritories = GameStateHelper.GetPlayerTerritoriesBitboard(in state, player);

        for (var i = 0; i < map.Continents.Length; i++)
        {
            var continentMask = map.ContinentMasks[i];

            // Single CPU register check: Does player own every territory in this continent?
            if ((playerTerritories & continentMask) == continentMask) totalTroops += map.Continents[i].BonusTroops;
        }

        return (byte)totalTroops;
    }
}