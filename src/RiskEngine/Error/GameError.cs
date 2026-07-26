public enum GameError : byte
{
    None = 0,

    InvalidAction = 1,
    ActionNotAllowedInPhase = 2,

    InvalidTerritory = 3,
    TerritoryNotOwned = 4,

    InvalidTroopCount = 5,
    NotEnoughTroops = 6,

    TerritoriesNotAdjacent = 7,
    CannotAttackOwnTerritory = 8,
    NotEnoughAttackTroops = 9,
    InvalidDiceCount = 10,
    InvalidTarget = 11,

    NoPathFound = 12,

    UnknownCard = 13,
    CardNotOwned = 14,
    InvalidCardSet = 15,


    InternalError = 255
}