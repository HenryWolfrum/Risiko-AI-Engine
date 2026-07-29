using System.Numerics;

namespace RiskEngine.State.Generation;

public static class FortifyActionGenerator
{
    /// <summary>
    /// Generates all valid (source, target) fortify pairs between adjacent owned territories.
    /// Iterates owned sources via bitboard and checks adjacent targets using MapLayout.
    /// </summary>
    public static int Generate(in GameState state, GameLayout layout, Span<GameAction> actions)
    {
        byte player = state.PlayerTurn;
        int actionCount = 0;

        // Bitmask of all territories owned by the active player
        ulong ownedTerritories = GameStateHelper.GetPlayerTerritoriesBitboard(in state, player);
        ulong sources = ownedTerritories;

        // Iterate over owned source territories
        while (sources != 0)
        {
            byte source = (byte)BitOperations.TrailingZeroCount(sources);

            // Source must have at least 2 troops to fortify (1 must remain behind)
            if (GameStateHelper.GetTerritoryTroops(in state, source) > 1)
            {
                // Retrieve array of neighbor territory IDs for the source territory
                byte[] neighbors = layout.Map.Adjacencies[source];

                for (int i = 0; i < neighbors.Length; i++)
                {
                    byte target = neighbors[i];

                    // O(1) Bitwise check: Is the target territory also owned by the player?
                    if ((ownedTerritories & (1UL << target)) != 0)
                    {
                        actions[actionCount++] = new GameAction
                        {
                            Type = ActionType.Fortify,
                            SourceTerritory = source,
                            TargetTerritory = target
                        };
                    }
                }
            }

            sources &= sources - 1;
        }

        // Ending the Turn is always legal Move in this phase
        actions[actionCount++] = new GameAction
        {
            Type = ActionType.EndTurn
        };
      

        return actionCount;
    }
}