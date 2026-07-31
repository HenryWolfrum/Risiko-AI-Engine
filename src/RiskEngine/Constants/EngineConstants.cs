namespace RiskEngine.State;

/// <summary>
/// Global engine limits and default values.
/// </summary>
public static class EngineConstants
{
    // ---------------------------------------------------------------------
    // Engine Limits
    // ---------------------------------------------------------------------

    /// <summary>
    /// Minimum supported player count.
    /// </summary>
    public const byte MIN_PLAYERS = 2;

    /// <summary>
    /// Maximum supported player count.
    /// </summary>
    public const byte MAX_PLAYERS = 6;

    /// <summary>
    /// Maximum supported territory count.
    /// Limited by the internal ulong bitboards.
    /// </summary>
    public const byte MAX_TERRITORIES = 64;

    public const int MAX_DECISION_BUFFER_SIZE = 2048;

    // ---------------------------------------------------------------------
    // Default Configuration
    // ---------------------------------------------------------------------

    public const byte DEFAULT_PLAYERS = 4;

    public const ushort MAX_ROUNDS = 200;

    public const byte JOKER_COUNT = 2;
    

    // ---------------------------------------------------------------------
    // Combat Rules
    // ---------------------------------------------------------------------

    public const byte MAX_ATTACKER_DICE = 3;

    public const byte MAX_DEFENDER_DICE = 2;


    // ---------------------------------------------------------------------
    // Reinforcement Rules
    // ---------------------------------------------------------------------

    public const byte MIN_REINFORCEMENT_TROOPS = 3;

    public const byte CARD_TERRITORY_BONUS_TROOPS = 2;

    public const byte FORCE_TRADE_CARD_COUNT = 5;
    
    
    // ---------------------------------------------------------------------
    // Fortify Rules
    // ---------------------------------------------------------------------
    
    public const byte MAX_FORTIFY_MOVES_PER_TURN = 1;
    
    // ---------------------------------------------------------------------
    // Misc
    // ---------------------------------------------------------------------

    public const byte NO_VALUE = byte.MaxValue;
}