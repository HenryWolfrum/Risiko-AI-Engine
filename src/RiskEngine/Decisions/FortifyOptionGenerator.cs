using System;
using System.Numerics;
using RiskEngine.Decisions;

namespace RiskEngine.State.Generation;

public static class FortifyOptionGenerator
{
    /// <summary>
    /// Generates all valid (source, target) fortify options for the active player.
    /// Finds connected clusters of owned territories using a highly optimized bitboard BFS,
    /// then creates parameterized options for every valid pair within each cluster.
    /// </summary>
    public static int Generate(in GameState state,GameLayout layout, Span<DecisionOption> options)
    {
        byte player = state.PlayerTurn;
        int optionCount = 0;

        // Bitmask of all territories currently owned by the active player
        ulong unvisitedOwned = GameStateHelper.GetPlayerTerritoriesBitboard(in state, player);

        // Stack-allocated BFS queue (max 64 territories on a standard Risk map)
        Span<byte> queue = stackalloc byte[EngineConstants.MAX_TERRITORIES];

        // 1. FIND CONNECTED COMPONENTS (CLUSTERS)
        while (unvisitedOwned != 0)
        {
            // Start a new cluster at the lowest unvisited territory
            byte startNode = (byte)BitOperations.TrailingZeroCount(unvisitedOwned);
            ulong clusterMask = 0;

            int head = 0;
            int tail = 0;

            // Enqueue start node and mark it as part of the current cluster
            queue[tail++] = startNode;
            ulong startMask = 1UL << startNode;
            clusterMask |= startMask;
            unvisitedOwned &= ~startMask; // Remove from unvisited pool

            // BFS to explore all reachable owned territories
            while (head < tail)
            {
                byte current = queue[head++];
                byte[] neighbors = layout.Map.Adjacencies[current];

                for (int i = 0; i < neighbors.Length; i++)
                {
                    byte neighbor = neighbors[i];
                    ulong neighborMask = 1UL << neighbor;

                    // If the neighbor is owned AND hasn't been added to a cluster yet
                    if ((unvisitedOwned & neighborMask) != 0)
                    {
                        clusterMask |= neighborMask;
                        unvisitedOwned &= ~neighborMask; // Mark visited
                        queue[tail++] = neighbor;
                    }
                }
            }

            // 2. GENERATE ALL VALID (SOURCE -> TARGET) PAIRS WITHIN THIS CLUSTER
            ulong sources = clusterMask;
            while (sources != 0)
            {
                byte source = (byte)BitOperations.TrailingZeroCount(sources);
                byte troops = GameStateHelper.GetTerritoryTroops(in state, source);

                // Source must have at least 2 troops to fortify (1 must remain behind)
                if (troops > 1)
                {
                    byte maxTroops = (byte)(troops - 1);
                    
                    // Targets are all OTHER territories in the SAME cluster
                    ulong targets = clusterMask & ~(1UL << source);
                    
                    while (targets != 0)
                    {
                        byte target = (byte)BitOperations.TrailingZeroCount(targets);
                        
                        options[optionCount++] = DecisionOption.Fortify(
                            source: source,
                            target: target,
                            minTroops: 1,
                            maxTroops: maxTroops
                        );

                        targets &= targets - 1; // Clear lowest set bit
                    }
                }

                sources &= sources - 1; // Clear lowest set bit
            }
        }

        // 3. ENDING THE TURN IS ALWAYS LEGAL
        // (Assuming you have an EndTurn factory method on DecisionOption)
        options[optionCount++] = DecisionOption.EndTurn();

        return optionCount;
    }
}