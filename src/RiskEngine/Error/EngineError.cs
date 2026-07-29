
public enum EngineError : byte
{
    None = 0,

    // =========================================
    // Generic Action Errors (1-19)
    // =========================================

    InvalidAction = 1,
    ActionNotAllowedInPhase = 2,

    // =========================================
    // Territory Errors (20-39)
    // =========================================

    InvalidTerritory = 20,
    TerritoryNotOwned = 21,
    InvalidTarget = 22,
    TerritoriesNotAdjacent = 23,
    NoPathFound = 24,

    // =========================================
    // Troop Errors (40-59)
    // =========================================

    InvalidTroopCount = 40,
    NotEnoughTroops = 41,
    NotEnoughAttackTroops = 42,

    // =========================================
    // Combat Errors (60-79)
    // =========================================

    CannotAttackOwnTerritory = 60,
    InvalidDiceCount = 61,

    // =========================================
    // Card Errors (80-99)
    // =========================================

    UnknownCard = 80,
    CardNotOwned = 81,
    InvalidCardSet = 82,

    // =========================================
// Layout Errors (100-159)
// =========================================

    InvalidLayout = 100,

// Cross Validation
    LayoutMismatch = 101,

// EngineConfig
    InvalidPlayerCount = 110,
    InvalidTerritoryCount = 111,
    InvalidMaxRounds = 112,

// Map
    DuplicateTerritoryName = 120,
    InvalidTerritoryName = 121,

    InvalidAdjacency = 122,
    MapNotConnected = 123,
    MapNotUndirected = 124,

    DuplicateContinentId = 125,
    DuplicateContinentName = 126,
    InvalidContinentId = 127,
    InvalidContinentName = 128,
    InvalidTerritoryToContinentMapping = 129,
    InvalidContinentTerritoryCount = 130,

// Deck
    InvalidCardCount = 140,
    InvalidCardType = 141,
    InvalidJokerCount = 142,
    InvalidJokerPosition = 143,
    InvalidCardTypeDistribution = 144,
    
    
    //Mission
    DuplicateMissionId = 145,
    InvalidMissionTerritoryTarget = 146,
    InvalidMissionContinent = 147,
    InvalidMissionPlayerTarget = 148,
    InvalidMissionType = 149,
    InvalidMissionIdSequence=150,
    
    // =========================================
    // Internal
    // =========================================

    InternalError = 255
}