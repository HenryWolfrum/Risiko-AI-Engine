namespace RiskEngine;

public static unsafe class MapTraverser
{
    public static bool HasPath(GameState state, MapLayout map, byte source, byte target, byte player)
    {
        //Source or Target do not belong to player
        if (state.GetTerritoryOwner(source) != player || state.GetTerritoryOwner(target) != player)
        {
            return false;
        }
        
        //Source is target
        if (source == target) return true;

        //visited stack array
        byte* visited = stackalloc byte[EngineConstants.DEFAULT_TERRITORY_COUNT];
        
        // BFS Queue on stack
        byte* queue = stackalloc byte[EngineConstants.DEFAULT_TERRITORY_COUNT];
        int head = 0;
        int tail = 0;

        // Start
        queue[tail++] = source;
        visited[source] = 1;

        
        while (head < tail)
        {
            byte current = queue[head++];

            // Traverse neighbors
            var neighbors = map.Adjacencies[current];
            for (byte i = 0; i < neighbors.Length; i++)
            {
                byte neighbor = neighbors[i];

                // if already visited -> skip
                if (visited[neighbor] == 1) continue;

                // target is reached
                if (neighbor == target)
                {
                    return true;
                }

                // owned neighbors added to queue
                if (state.GetTerritoryOwner(neighbor) == player)
                {
                    visited[neighbor] = 1;
                    queue[tail++] = neighbor;
                }
            }
        }

        //Nothing found
        return false;
    }
}