namespace RiskEngine;

public struct GameState
{
    // Territory Info
    public byte[] TerritoryOwners;
    public byte[] TerritoryTroops;

    // Player Info
    public byte[] PlayerTroopsToPlace;
    public byte[][] PlayersCards;
    public bool[] IsPlayerAlive;

    // Card Deck
    public ulong CardDeckBitboard;

    // Historical Steps
    public ushort CurrentRound;
    public byte PlayerTurn;
    public GamePhase CurrentPhase;

    public bool HasConqueredTerritoryThisTurn;

    public GameState(EngineConfig config)
    {
        TerritoryOwners = new byte[config.TerritoryCount];
        TerritoryTroops = new byte[config.TerritoryCount];

        PlayerTroopsToPlace = new byte[config.PlayerCount];
        PlayersCards = new byte[config.PlayerCount][];
        IsPlayerAlive = new bool[config.PlayerCount];

        for (int i = 0; i < config.PlayerCount; i++)
        {
            IsPlayerAlive[i] = true;
            PlayersCards[i] = Array.Empty<byte>();
        }

        CardDeckBitboard = 0;
        PlayerTurn = 0;
        CurrentPhase = GamePhase.Placement;
        CurrentRound = 1;
        HasConqueredTerritoryThisTurn = false;
    }
}