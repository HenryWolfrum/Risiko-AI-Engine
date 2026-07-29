namespace RiskEngine.Mission;

public readonly struct MissionDefinition
{
    public byte Id { get; init; }
    public MissionType Type { get; init; }
    
    // Territory mission parameters
    public byte RequiredTerritories { get; init; }
    public byte MinTroopsPerTerritory { get; init; }
    
    // Continent mission parameters
    public byte TargetContinentMask { get; init; } 
    public bool RequiresThirdContinentChoice { get; init; }
    
    // Elimination mission parameters       // #p1 ; #p2 ; #p3 ; #p4; #p5; 24 GB ; 18GB � 2E ; NA-AU; NA-AF; AS-AF; AS-SA ; EU-AU
    public byte TargetPlayerId { get; init; }
}