namespace RiskEngine.Layout;

/// <summary>
/// Represents a player action with fixed fields to cover all scenarios
/// </summary>
public struct GameAction
{
    public ActionType Type;

   
    public byte Card1;
    public byte Card2;
    public byte Card3;

   
    public byte TroopCount;

  
    public byte ChosenAttackerDiceCount;
    public byte ChosenDefenderDiceCount;

    public byte SourceTerritory;
    public byte TargetTerritory;


    
    public GameAction()
    {
        //Marks them as Engine valid but set to no value
        byte noValue = EngineConstants.NO_VALUE;
        Type = ActionType.NotInitialized;
        
        Card1 = noValue;
        Card2 = noValue;
        Card3 = noValue;
        
        TroopCount = noValue;
        
        ChosenAttackerDiceCount = noValue;
        ChosenDefenderDiceCount = noValue;
        
        SourceTerritory = noValue;
        TargetTerritory = noValue;
    }


}