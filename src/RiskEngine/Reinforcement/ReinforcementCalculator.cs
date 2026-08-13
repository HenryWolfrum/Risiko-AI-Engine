using System.Runtime.CompilerServices;

namespace RiskEngine.State;

public static class ReinforcementCalculator
{
    /// <summary>
    ///     Calculates base reinforcements + continent bonuses for the active player at turn start.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte CalculateTroops(in GameState state, MapLayout map,byte playerId)
    {
    
        //Reinforcement troops are calculated by the base troops + additional Continent Boni
        return (byte)(CalculateBaseTroops(in state, playerId) + CalculateContinentBonusTroops(in state, in map, playerId));
        
    }

    public static byte CalculateBaseTroops(in GameState state,byte playerId)
    {
        // 1. Base income: floor(owned / 3) with a minimum threshold of usually 3
        byte ownedCount =(byte) GameStateHelper.GetOwnedTerritoryCount(in state, playerId);
        
        return (byte) Math.Max(EngineConstants.MIN_REINFORCEMENT_TROOPS, ownedCount / 3);
      
    }

    public static byte CalculateContinentBonusTroops(in GameState state, in MapLayout map, byte playerId)
    {
        // 2. Continent bonus check via precalculated Bitboards
        byte continentBonus = 0;
        
        var playerTerritories = GameStateHelper.GetPlayerTerritoriesBitboard(in state, playerId);

        for (var i = 0; i < map.Continents.Length; i++)
        {
            var continentMask = map.ContinentMasks[i];

            // Single CPU register check: Does player own every territory in this continent?
            if ((playerTerritories & continentMask) == continentMask) continentBonus+= map.Continents[i].BonusTroops;
        }

        return (byte)continentBonus;
    }
}