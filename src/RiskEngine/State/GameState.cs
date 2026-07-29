namespace RiskEngine.State;

public unsafe struct GameState
{
    // --- Territory Info ---
    public fixed byte TerritoryOwners[EngineConstants.MAX_TERRITORIES];
    public fixed byte TerritoryTroops[EngineConstants.MAX_TERRITORIES];

    // --- Player Info ---
    public fixed byte PlayerTroopsToPlace[EngineConstants.MAX_PLAYERS];
    public fixed byte PlayerMissions[EngineConstants.MAX_PLAYERS];

    // --- Cards & Deck Info ---
    public fixed ulong PlayerCardsBitboard[EngineConstants.MAX_PLAYERS];
    public byte CardSetsTradedCount;

    // --- History / Game Loop ---
    public ushort CurrentRound;
    public byte PlayerTurn;
    public GamePhase CurrentPhase;

    public byte PlayersAliveBitboard;
    
    
    public byte WinnerId;
}