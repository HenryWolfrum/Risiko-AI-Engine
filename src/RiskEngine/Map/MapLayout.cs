namespace RiskEngine;

// Abstract Scheme of a Risk based Map
public class MapLayout
{
    // Static list arrays
    public readonly string[] TerritoryNames;
    public readonly byte[][] Adjacencies;
    public readonly byte[] TerritoryToContinent;
    public readonly Continent[] Continents;

    // Precalculated Bitmasks for O(1) continent control checks
    public readonly ulong[] ContinentMasks;

    public MapLayout(string[] territoryNames, byte[][] adjacencies, byte[] territoryToContinentMap, Continent[] continents)
    {
        TerritoryNames = territoryNames;
        Adjacencies = adjacencies;
        TerritoryToContinent = territoryToContinentMap;
        Continents = continents;

        // Precalculate bitmasks for each continent
        ContinentMasks = new ulong[continents.Length];
        for (int i = 0; i < territoryToContinentMap.Length; i++)
        {
            byte continentId = territoryToContinentMap[i];
            ContinentMasks[continentId] |= (1UL << i);
        }
    }

    // Low Level check for Neighbors
    public bool AreNeighbors(byte territoryA, byte territoryB)
    {
        byte[] neighbors = Adjacencies[territoryA];

        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] == territoryB)
            {
                return true;
            }
        }

        return false;
    }

    public Continent GetContinentOfTerritory(byte territoryId)
    {
        byte continentId = TerritoryToContinent[territoryId];
        return Continents[continentId];
    }
}