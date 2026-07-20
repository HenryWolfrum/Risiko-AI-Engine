namespace RiskEngine;

public static class RiskMapFactory
{
    public static MapLayout CreateStandardRiskMap()
    {
        //Define Continents
        var continents = new Continent[]
        {
            new Continent(id: 0, name: "Nordamerika", bonusTroops: 5, territoryCount: 9),
            new Continent(id: 1, name: "Südamerika",  bonusTroops: 2, territoryCount: 4),
            new Continent(id: 2, name: "Europa",      bonusTroops: 5, territoryCount: 7),
            new Continent(id: 3, name: "Afrika",      bonusTroops: 3, territoryCount: 6),
            new Continent(id: 4, name: "Asien",       bonusTroops: 7, territoryCount: 12),
            new Continent(id: 5, name: "Australien",  bonusTroops: 2, territoryCount: 4)
        };

        //Names
        string[] territoryNames = new string[]
        {
            "Alaska", "Nordwest-Territorium", "Grönland" // ...
        };

        //TerritoryId -> ContinentId
        byte[] territoryToContinent = new byte[]
        {
            0, 0, 0 //...
        };

        //Adjaceny List for Neighboring Territories
        byte[][] adjacencies = new byte[][]
        {
            new byte[] { 1, 37 }, // Alaska (0) grenzt an Nordwest-Territorium (1) und Kamtschatka (37)
            new byte[] { 0, 2 }  // Nordwest (1) grenzt an Alaska (0) ...
            //...
        };

      
        return new MapLayout(territoryNames, adjacencies, territoryToContinent, continents);
    }
}