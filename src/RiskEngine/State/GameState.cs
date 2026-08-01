namespace RiskEngine.State;

public unsafe struct GameState
{
    // --- Bitboards (8-Byte Aligned for optimal Memory Layout) ---
    public fixed ulong PlayerTerritoriesBitboard[EngineConstants.MAX_PLAYERS];
    public fixed ulong PlayerCardsBitboard[EngineConstants.MAX_PLAYERS];

    // --- Territory Info ---
    public fixed byte TerritoryOwners[EngineConstants.MAX_TERRITORIES];
    public fixed byte TerritoryTroops[EngineConstants.MAX_TERRITORIES];

    // --- Player Info ---
    public fixed byte PlayerTroopsToPlace[EngineConstants.MAX_PLAYERS];
    public fixed byte PlayerMissions[EngineConstants.MAX_PLAYERS];

    // --- History / Game Loop ---
    public ushort CurrentRound;
    public byte PlayerTurn;
    public GamePhase CurrentPhase;

    public byte PlayersAliveBitboard;
    public byte CardSetsTradedCount;

    public byte AttackerTerritory;
    public byte DefenderTerritory;
    
    public byte WinnerId;
}