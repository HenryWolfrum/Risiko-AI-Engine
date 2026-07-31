namespace RiskEngine.State;

//Generic GameAction 
public struct GameAction
{
    public ActionType Type;

    //For Card Retrieval
    public byte Card1;
    public byte Card2;
    public byte Card3;

    //Reinforce Troops/Conquer
    public byte TroopCount;

    //For Attack/Fortify
    public byte ChosenAttackerDiceCount;
    public byte ChosenDefenderDiceCount;

    public byte SourceTerritory;
    public byte TargetTerritory;


    public GameAction()
    {
        byte noValue = EngineConstants.NO_VALUE;
        Type = ActionType.Default;
        
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