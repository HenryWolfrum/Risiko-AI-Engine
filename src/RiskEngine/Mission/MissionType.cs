namespace RiskEngine.Mission;

public enum MissionType : byte
{
    WorldDomination = 0,    // Dynamic: Control all territories on the map
    ConquerTerritories = 1, // Control X territories (e.g., 24 territories or 18 with at least 2 troops each)
    ConquerContinents = 2,  // Control specific continents via bitmask
    EliminatePlayer = 3     // Eliminate a specific target player (Fallback: 24 territories)
}