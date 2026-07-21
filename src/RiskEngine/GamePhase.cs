namespace RiskEngine;

public enum GamePhase : byte
{
    Default = 0,   // Uninitialized / Config
    Start = 1,     // Game Start allowed
    Init = 2,      // Setup Game
    Bonus = 3,     // Card Bonus Retrieval (optional/forced)
    Reinforce = 4, // Reinforce owned Territories
    Attack = 5,    // Attack Phase
    Conquer = 6,   // Troop Invasion after successful Conquer
    Move = 7,      // Troop Distributing
    End = 8,       // Game decided

    Error = 255    // Error State
}
