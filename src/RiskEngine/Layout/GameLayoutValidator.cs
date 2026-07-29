namespace RiskEngine.State;

public static class GameLayoutValidator
{
    public static ValidationResult Validate(MapLayout map, DeckLayout deck, EngineConfig config)
    {
        var result = ValidateConfig(config);
        if (!result.IsValid)
            return result;

        result = ValidateMap(map);
        if (!result.IsValid)
            return result;
        
        result = ValidateContinents(map);
        if (!result.IsValid)
            return result;

        result = ValidateDeck(deck);
        if (!result.IsValid)
            return result;

        result = ValidateCrossLayout(map, deck);
        if (!result.IsValid)
            return result;

        return ValidationResult.Valid();
    }

    private static ValidationResult ValidateConfig(EngineConfig config)
    {
        //PlayerCount
        if (config.PlayerCount < EngineConstants.MIN_PLAYERS || config.PlayerCount > EngineConstants.MAX_PLAYERS)
        {
            return ValidationResult.Invalid(EngineError.InvalidPlayerCount);
        }

        if (config.MaxRounds == 0 || config.MaxRounds > EngineConstants.MAX_ROUNDS)
        {
            return ValidationResult.Invalid(EngineError.InvalidMaxRounds);
        }

        return ValidationResult.Valid();
    }

   private static ValidationResult ValidateMap(MapLayout map)
{
    // ==========================================
    // 1. TerritoryCount
    // ==========================================

    if (map.TerritoryCount == 0)
        return ValidationResult.Invalid(EngineError.InvalidTerritoryCount);

    if (map.TerritoryCount > EngineConstants.MAX_TERRITORIES)
        return ValidationResult.Invalid(EngineError.InvalidTerritoryCount);

    if (map.TerritoryNames.Length != map.TerritoryCount)
        return ValidationResult.Invalid(EngineError.InvalidTerritoryCount);

    if (map.Adjacencies.Length != map.TerritoryCount)
        return ValidationResult.Invalid(EngineError.InvalidTerritoryCount);


    // ==========================================
    // 2. Territory Names
    // ==========================================

    var existingNames = new HashSet<string>();

    for (int territory = 0; territory < map.TerritoryCount; territory++)
    {
        var name = map.TerritoryNames[territory];

        if (string.IsNullOrWhiteSpace(name))
            return ValidationResult.Invalid(EngineError.InvalidTerritoryName);

        if (!existingNames.Add(name))
            return ValidationResult.Invalid(EngineError.InvalidTerritoryName);
    }


    // ==========================================
    // 3. Adjacency Lists
    // ==========================================

    for (byte territory = 0; territory < map.TerritoryCount; territory++)
    {
        var neighbours = map.Adjacencies[territory];

        if (neighbours == null)
            return ValidationResult.Invalid(EngineError.InvalidAdjacency);

        var visitedNeighbours = new HashSet<byte>();

        foreach (var neighbour in neighbours)
        {
            // Territory must exist
            if (neighbour >= map.TerritoryCount)
                return ValidationResult.Invalid(EngineError.InvalidAdjacency);

            // No self-loop
            if (neighbour == territory)
                return ValidationResult.Invalid(EngineError.InvalidAdjacency);

            // No duplicate neighbours
            if (!visitedNeighbours.Add(neighbour))
                return ValidationResult.Invalid(EngineError.InvalidAdjacency);
        }
    }

    
    // ==========================================
    // 4. Map must be connected
    // ==========================================
    
    if (!MapTraverser.IsConnected(map))
    {
        return ValidationResult.Invalid(EngineError.MapNotConnected);
    }
    
    // ==========================================
    // 5. Map must be undirected
    // ==========================================

    if (!MapTraverser.IsUndirected(map))
    {
        return ValidationResult.Invalid(EngineError.MapNotUndirected);
    }
    

    return ValidationResult.Valid();
}
   
       private static ValidationResult ValidateContinents(MapLayout map)
    {
        // Keine Kontinente
        if (map.Continents == null || map.Continents.Length == 0)
            return ValidationResult.Invalid(EngineError.InvalidContinentId);
    
        var ids = new HashSet<byte>();
        var names = new HashSet<string>();
    
        var territoryCounter = new int[map.Continents.Length];
    
        // ----------------------------
        // Kontinente prüfen
        // ----------------------------
        foreach (var continent in map.Continents)
        {
            if (!ids.Add(continent.Id))
                return ValidationResult.Invalid(EngineError.DuplicateContinentId);
    
            if (string.IsNullOrWhiteSpace(continent.Name))
                return ValidationResult.Invalid(EngineError.DuplicateContinentName);
    
            if (!names.Add(continent.Name))
                return ValidationResult.Invalid(EngineError.DuplicateContinentName);
        }
    
        // IDs müssen 0..N-1 sein
        for (byte i = 0; i < map.Continents.Length; i++)
        {
            if (map.Continents[i].Id != i)
                return ValidationResult.Invalid(EngineError.InvalidContinentId);
        }
    
        // ----------------------------
        // Territory -> Continent Mapping
        // ----------------------------
        if (map.TerritoryToContinent.Length != map.TerritoryCount)
            return ValidationResult.Invalid(EngineError.InvalidTerritoryToContinentMapping);
    
        for (int territory = 0; territory < map.TerritoryCount; territory++)
        {
            var continentId = map.TerritoryToContinent[territory];
    
            if (continentId >= map.Continents.Length)
                return ValidationResult.Invalid(EngineError.InvalidTerritoryToContinentMapping);
    
            territoryCounter[continentId]++;
        }
    
        // ----------------------------
        // TerritoryCount je Kontinent
        // ----------------------------
        int total = 0;
    
        for (int i = 0; i < map.Continents.Length; i++)
        {
            if (territoryCounter[i] != map.Continents[i].TerritoryCount)
                return ValidationResult.Invalid(EngineError.InvalidContinentTerritoryCount);
    
            total += territoryCounter[i];
        }
    
        if (total != map.TerritoryCount)
            return ValidationResult.Invalid(EngineError.InvalidContinentTerritoryCount);
    
        return ValidationResult.Valid();
    }

    private static ValidationResult ValidateDeck(DeckLayout deck)
    {
        // ----------------------------
        // 1. Basic Checks
        // ----------------------------
    
        if (deck.TerritoryToType == null)
            return ValidationResult.Invalid(EngineError.InvalidCardCount);
    
        if (deck.CardCount == 0 || deck.CardCount > EngineConstants.MAX_TERRITORIES + EngineConstants.JOKER_COUNT)
            return ValidationResult.Invalid(EngineError.InvalidCardCount);
        
    
        // ----------------------------
        // 2. Count Card Types
        // ----------------------------
    
        int infantry = 0;
        int cavalry = 0;
        int artillery = 0;
        int jokers = 0;
    
        for (int i = 0; i < deck.CardCount; i++)
        {
            switch (deck.TerritoryToType[i])
            {
                case CardType.Infantry:
                    infantry++;
                    break;
    
                case CardType.Cavalry:
                    cavalry++;
                    break;
    
                case CardType.Artillery:
                    artillery++;
                    break;
    
                case CardType.Joker:
                    jokers++;
                    break;
    
                default:
                    return ValidationResult.Invalid(EngineError.InvalidCardType);
            }
        }
    
        // ----------------------------
        // 3. Joker Count
        // ----------------------------
    
        if (jokers != EngineConstants.JOKER_COUNT)
            return ValidationResult.Invalid(EngineError.InvalidJokerCount);
    
        // ----------------------------
        // 4. Joker Position
        // ----------------------------
    
        for (int i = 0; i < deck.CardCount - EngineConstants.JOKER_COUNT; i++)
        {
            if (deck.TerritoryToType[i] == CardType.Joker)
                return ValidationResult.Invalid(EngineError.InvalidJokerPosition);
        }
    
        for (int i = deck.CardCount - EngineConstants.JOKER_COUNT; i < deck.CardCount; i++)
        {
            if (deck.TerritoryToType[i] != CardType.Joker)
                return ValidationResult.Invalid(EngineError.InvalidJokerPosition);
        }
    
        // ----------------------------
        // 5. Balanced Distribution
        // ----------------------------
    
        int min = Math.Min(infantry, Math.Min(cavalry, artillery));
        int max = Math.Max(infantry, Math.Max(cavalry, artillery));
    
        if (max - min > 1)
            return ValidationResult.Invalid(EngineError.InvalidCardTypeDistribution);
    
        return ValidationResult.Valid();
    }

    private static ValidationResult ValidateCrossLayout(MapLayout map, DeckLayout deck)
    {
        if (deck.TerritoryCardCount != map.TerritoryCount)
        {
            return ValidationResult.Invalid(EngineError.LayoutMismatch);
        }
        

        return ValidationResult.Valid();
    }
}