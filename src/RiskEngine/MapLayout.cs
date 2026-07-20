namespace RiskEngine;


//Abstract Scheme of a Risk based Map
public class MapLayout
{
    //static list arrays
    public readonly string[] TerritoryNames;
    public readonly byte[][] Adjacencies;
    public readonly byte[] TerritoryToContinentMap;

  
    public readonly Continent[] Continents;

    public MapLayout(
        string[] territoryNames,
        byte[][] adjacencies,
        byte[] territoryToContinentMap,
        Continent[] continents)
    {
        TerritoryNames = territoryNames;
        Adjacencies = adjacencies;
        TerritoryToContinentMap = territoryToContinentMap;
        Continents = continents;
    }

  
    //Low Level check for Neighbors
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
        byte continentId = TerritoryToContinentMap[territoryId];
        return Continents[continentId];
    }
}