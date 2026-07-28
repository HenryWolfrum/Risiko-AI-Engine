namespace RiskEngine;

public static unsafe class MapTraverser
{
    public static bool HasPath(GameState state, MapLayout map, byte source, byte target, byte player)
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
}