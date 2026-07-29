using System.Numerics;

namespace RiskEngine.State.Generation;

public static class AttackActionGenerator
{
    /// <summary>
    /// Generates all valid attack target pairs (Source -> Target) for the active player.
    /// Attacker dice count is assumed optimal (max available), and defender choices
    /// are resolved dynamically inside the AttackExecutor loop.
    /// </summary>
    public static int Generate(in GameState state, GameLayout layout, Span<GameAction> actions)
    {
        byte player = state.PlayerTurn;
        int actionCount = 0;

        // Bitmask of all territories owned by the active player
        ulong ownedTerritories = GameStateHelper.GetPlayerTerritoriesBitboard(in state, player);
        ulong sources = ownedTerritories;

        // Iterate over owned source territories via Bitboard
        while (sources != 0)
        {
            byte source = (byte)BitOperations.TrailingZeroCount(sources);

            // Attacker needs at least 2 troops to attack (1 troop must stay behind)
            if (GameStateHelper.GetTerritoryTroops(in state, source) > 1)
            {
                byte[] neighbors = layout.Map.Adjacencies[source];

                for (int i = 0; i < neighbors.Length; i++)
                {
                    byte target = neighbors[i];

                    // Target MUST belong to an opponent (NOT in ownedTerritories)
                    if ((ownedTerritories & (1UL << target)) == 0)
                    {
                        actions[actionCount++] = new GameAction
                        {
                            Type = ActionType.Attack,
                            SourceTerritory = source,
                            TargetTerritory = target
                        };
                    }
                }
            }

            sources &= sources - 1;
        }

        // Ending the attack phase voluntarily is always a valid action
        actions[actionCount++] = new GameAction
        {
            Type = ActionType.SkipPhase
        };

        return actionCount;
    }
}