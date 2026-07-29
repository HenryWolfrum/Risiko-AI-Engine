namespace RiskEngine.State;

public static unsafe class MapTraverser
{
    public static bool HasPath(in GameState state, MapLayout map, byte source, byte target, byte player)
    {
        //Source or Target do not belong to player
        if (GameStateHelper.GetTerritoryOwner(state, source) != player ||
            GameStateHelper.GetTerritoryOwner(state, target) != player) return false;

        //Source is target
        if (source == target) return true;

        //visited stack array
        var visited = stackalloc byte[map.TerritoryCount];

        // BFS Queue on stack
        var queue = stackalloc byte[map.TerritoryCount];
        var head = 0;
        var tail = 0;

        // Start
        queue[tail++] = source;
        visited[source] = 1;


        while (head < tail)
        {
            var current = queue[head++];

            // Traverse neighbors
            var neighbors = map.Adjacencies[current];
            for (byte i = 0; i < neighbors.Length; i++)
            {
                var neighbor = neighbors[i];

                // if already visited -> skip
                if (visited[neighbor] == 1) continue;

                // target is reached
                if (neighbor == target) return true;

                // owned neighbors added to queue
                if (GameStateHelper.GetTerritoryOwner(state, neighbor) == player)
                {
                    visited[neighbor] = 1;
                    queue[tail++] = neighbor;
                }
            }
        }

        //Nothing found
        return false;
    }
    
    
    public static bool IsConnected(MapLayout map)
    {
        // Empty maps are considered invalid by the LayoutValidator.
        if (map.TerritoryCount == 0)
            return false;

        // Visited array
        var visited = stackalloc byte[map.TerritoryCount];

        // BFS queue
        var queue = stackalloc byte[map.TerritoryCount];
        var head = 0;
        var tail = 0;

        // Start traversal at territory 0
        queue[tail++] = 0;
        visited[0] = 1;

        while (head < tail)
        {
            var current = queue[head++];

            var neighbours = map.Adjacencies[current];

            for (byte i = 0; i < neighbours.Length; i++)
            {
                var neighbour = neighbours[i];

                if (visited[neighbour] == 1)
                    continue;

                visited[neighbour] = 1;
                queue[tail++] = neighbour;
            }
        }

        // Verify that every territory has been reached
        for (byte i = 0; i < map.TerritoryCount; i++)
        {
            if (visited[i] == 0)
                return false;
        }

        return true;
    }
    
    
    public static bool IsUndirected(MapLayout map)
    {
        for (byte territory = 0; territory < map.TerritoryCount; territory++)
        {
            foreach (var neighbour in map.Adjacencies[territory])
            {
                bool found = false;

                foreach (var reverse in map.Adjacencies[neighbour])
                {
                    if (reverse == territory)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    return false;
            }
        }

        return true;
    }
}