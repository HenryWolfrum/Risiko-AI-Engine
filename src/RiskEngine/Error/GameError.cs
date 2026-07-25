public enum GameError : byte
{
    None = 0,

    InvalidAction = 1,
    ActionNotAllowedInPhase = 2,

    InvalidTerritory = 3,
    TerritoryNotOwned = 4,

    InvalidTroopCount = 5,
    NotEnoughTroops = 6,

    InternalError = 255
}