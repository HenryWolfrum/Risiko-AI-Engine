namespace RiskEngine;

public static class RiskMapFactory
{
    public static GameLayout CreateStandardRiskMap(EngineConfig? config=null)
    {
        //Check for null config and set default values if necessary
        EngineConfig gameConfig = config ?? new EngineConfig(EngineConstants.DEFAULT_PLAYERS, EngineConstants.MAX_ROUNDS);

        //Define Continents
        var continents = new Continent[]
        {
            new Continent(id: 0, name: "Nordamerika", bonusTroops: 5, territoryCount: 9),
            new Continent(id: 1, name: "Südamerika",  bonusTroops: 2, territoryCount: 4),
            new Continent(id: 2, name: "Afrika",      bonusTroops: 3, territoryCount: 6),
            new Continent(id: 3, name: "Europa",      bonusTroops: 5, territoryCount: 7),
            new Continent(id: 4, name: "Asien",       bonusTroops: 7, territoryCount: 12),
            new Continent(id: 5, name: "Australien",  bonusTroops: 2, territoryCount: 4)
        };

        //Names
        string[] territoryNames = new string[]
        {
            //North-America
            "Alaska",
            "Nordwest-Territorium",
            "Alberta",
            "Ontario",
            "Ostkanada",
            "Weststaaten",
            "Oststaaten",
            "Grönland",
            "Mittelamerika",
            
            //South-America
            "Venezuela",
            "Peru",
            "Brasilien",
            "Argentinien",
            
            //Africa
            "Nordafrika",
            "Ägypten",
            "Zentralafrika",
            "Ostafrika",
            "Madagaskar",
            "Südafrika",
            
            //Europe
            "Westeuropa",
            "Südeuropa",
            "Nordeuropa",
            "Skandinavien",
            "Großbritannien",
            "Island",
            "Russland",
            
            //Asia
            "Ural",
            "Sibirien",
            "Jakutien",
            "Kamtschatka",
            "Irkutsk",
            "Mongolei",
            "China",
            "Südostasien",
            "Indien",
            "Afghanistan",
            "Naher Osten",
            "Japan",
            
            //Australia
            "Indonesien",
            "Neu Guinea",
            "Westaustralien",
            "Ostaustralien"
        };

        //TerritoryId -> ContinentId
        byte[] territoryToContinent = new byte[]
        {
            0,0,0,0,0,0,0,0,0, // North America
            1,1,1,1,         // South America
            2,2,2,2,2,2,     // Africa
            3,3,3,3,3,3,3,   // Europe
            4,4,4,4,4,4,4,4,4,4,4,4, // Asia
            5,5,5,5          // Australia
        };

        //Neighboring Territories (Adjacency List)
        byte[][] adjacencies = new byte[][]
        {
            //North-America
            new byte[] { 1,2,29 },
            new byte[] { 0,2,3,7 },
            new byte[] { 0,1,3,5 },
            new byte[] { 1,2,4,5,6,7 },
            new byte[] { 3,6,7 },
            new byte[] { 2,3,6,8 },
            new byte[] { 3,4,5,8 },
            new byte[] { 1,3,4,24 },
            new byte[] { 5,6,9 },

            //South-America
            new byte[] { 8,10,11 },
            new byte[] { 9,11,12 },
            new byte[] { 9,10,12,13 },
            new byte[] { 10,11 },

            //Africa
            new byte[] { 11,14,15,16,19,20 },
            new byte[] { 13,16,20,36 },
            new byte[] { 13,16,18 },
            new byte[] { 13,14,15,17,18,36 },
            new byte[] { 16,18 },
            new byte[] { 15,16,17 },

            //Europe
            new byte[] { 13,20,21,23 },
            new byte[] { 13,14,19,21,25,36 },
            new byte[] { 19,20,22,23,25 },
            new byte[] { 21,23,24,25 },
            new byte[] { 19,21,22,24 },
            new byte[] { 7,22,23 },
            new byte[] { 20,21,22,26,35,36 },

            //Asia
            new byte[] { 25,27,32,35 },
            new byte[] { 26,28,30,31,32 },
            new byte[] { 27,29,30 },
            new byte[] { 0,28,30,31,37 },
            new byte[] { 27,28,29,31 },
            new byte[] { 27,29,30,32,37 },
            new byte[] { 26, 27, 31, 33, 34, 35 },
            new byte[] { 32,34,38 },
            new byte[] { 32,33,35,36 },
            new byte[] { 25,26,32,34,36 },
            new byte[]{ 14, 16, 20, 25, 34, 35 },
            new byte[] { 29,31 },

            //Australia
            new byte[] { 33,39,40 },
            new byte[] { 38,41 },
            new byte[] { 38,41 },
            new byte[] { 39,40 }
            
        };


        //TerritoryId -> CardType
        CardType[] territoryToType = new CardType[]
        {
            // North-America (9)
            CardType.Infantry, CardType.Artillery, CardType.Cavalry, CardType.Cavalry, CardType.Cavalry,
            CardType.Artillery, CardType.Artillery, CardType.Cavalry, CardType.Artillery,
            // South-America (4)
            CardType.Infantry, CardType.Infantry, CardType.Artillery, CardType.Infantry,
            // Africa (6)
            CardType.Cavalry, CardType.Infantry, CardType.Infantry, CardType.Infantry, CardType.Cavalry, CardType.Artillery,
            // Europe (7)
            CardType.Artillery, CardType.Artillery, CardType.Artillery, CardType.Cavalry, CardType.Artillery, CardType.Infantry, CardType.Cavalry,
            // Asia (12)
            CardType.Cavalry, CardType.Cavalry, CardType.Cavalry, CardType.Infantry, CardType.Cavalry, CardType.Infantry,
            CardType.Infantry, CardType.Infantry, CardType.Cavalry, CardType.Cavalry, CardType.Infantry, CardType.Artillery,
            // Australia (4)
            CardType.Artillery, CardType.Infantry, CardType.Artillery, CardType.Artillery,

            //Joker (2)
            CardType.Joker, CardType.Joker        
        };


        MapLayout mapLayout = new MapLayout(territoryNames, adjacencies, territoryToContinent, continents);
        DeckLayout deckLayout = new DeckLayout(territoryToType);

        return new GameLayout(mapLayout, deckLayout,gameConfig);
    }
}