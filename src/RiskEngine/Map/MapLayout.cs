namespace RiskEngine;

// Abstract Scheme of a Risk based Map
public class MapLayout
{
    public readonly byte[][] Adjacencies;

    // Precalculated Bitmasks for O(1) continent control checks
    public readonly ulong[] ContinentMasks;

    public readonly Continent[] Continents;

    // Static list arrays
    public readonly string[] TerritoryNames;
    public readonly byte[] TerritoryToContinent;
    
    public byte TerritoryCount { get; }

    public MapLayout(string[] territoryNames, byte[][] adjacencies, byte[] territoryToContinentMap,
        Continent[] continents)
    {
        TerritoryNames = territoryNames;
        Adjacencies = adjacencies;
        TerritoryToContinent = territoryToContinentMap;
        Continents = continents;

        TerritoryCount = (byte)territoryNames.Length;

        // Precalculate bitmasks for each continent
        ContinentMasks = new ulong[continents.Length];

        for (var i = 0; i < territoryToContinentMap.Length; i++)
        {
            var continentId = territoryToContinentMap[i];
            ContinentMasks[continentId] |= 1UL << i;
        }
    }

    // Low Level check for Neighbors
    public bool AreNeighbors(byte territoryA, byte territoryB)
    {
        var neighbors = Adjacencies[territoryA];

        for (var i = 0; i < neighbors.Length; i++)
            if (neighbors[i] == territoryB)
                return true;

        return false;
    }

    public Continent GetContinentOfTerritory(byte territoryId)
    {
        var continentId = TerritoryToContinent[territoryId];
        return Continents[continentId];
    }
}