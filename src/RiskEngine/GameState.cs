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

    // Deck Info
    public ulong CardDeckBitboard;
    public byte CardSetsTradedCount;

    // History steps
    public ushort CurrentRound;
    public byte PlayerTurn;
    public GamePhase CurrentPhase;

    // GamePhase: Conquer
    public bool HasConqueredTerritoryThisTurn;

    // GamePhase: Attack
    public byte SelectedAttackerTerritory;
    public byte SelectedDefenderTerritory;

    // GamePhase: Fortify / Move
    public byte SelectedFortifySource;
    public byte SelectedFortifyTarget;

    // Last Dice Info
    public byte LastAttackerDiceCount;
    public byte LastDefenderDiceCount;
    public byte[] LastAttackerDiceValues;
    public byte[] LastDefenderDiceValues;

    /// <summary>
    /// Initializes a new instance of the GameState struct with default values based on the provided EngineConfig.
    /// </summary>
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
        CardSetsTradedCount = 0;

        PlayerTurn = 0;
        CurrentPhase = GamePhase.Default;
        CurrentRound = 1;
        HasConqueredTerritoryThisTurn = false;

        SelectedAttackerTerritory = EngineConstants.NO_VALUE;
        SelectedDefenderTerritory = EngineConstants.NO_VALUE;

        SelectedFortifySource = EngineConstants.NO_VALUE;
        SelectedFortifyTarget = EngineConstants.NO_VALUE;

        LastAttackerDiceCount = EngineConstants.NO_VALUE;
        LastDefenderDiceCount = EngineConstants.NO_VALUE;

        LastAttackerDiceValues = Array.Empty<byte>();
        LastDefenderDiceValues = Array.Empty<byte>();
    }

    /// <summary>
    /// Setzt den Phasen-Kontext beim Wechsel von Phasen (z. B. Attack -> Fortify) zurück.
    /// </summary>
    public void ResetPhaseContext()
    {
        SelectedAttackerTerritory = EngineConstants.NO_VALUE;
        SelectedDefenderTerritory = EngineConstants.NO_VALUE;

        SelectedFortifySource = EngineConstants.NO_VALUE;
        SelectedFortifyTarget = EngineConstants.NO_VALUE;

        LastAttackerDiceCount = EngineConstants.NO_VALUE;
        LastDefenderDiceCount = EngineConstants.NO_VALUE;
        LastAttackerDiceValues = Array.Empty<byte>();
        LastDefenderDiceValues = Array.Empty<byte>();
    }
}