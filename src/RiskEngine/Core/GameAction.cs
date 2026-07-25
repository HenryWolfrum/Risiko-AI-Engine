namespace RiskEngine;

//Generic GameAction 
public struct GameAction
{
    public ActionType Type;
    
    //For Card Retrieval
    public CardType Card1;
    public CardType Card2;
    public CardType Card3;
    
    //For placing Troops
    public byte TroopCount;
    
    //For Attack/Fortify
    public byte ChosenAttackerDiceCount;
    public byte ChosenDefenderDiceCount;
    
    public byte SourceTerritory;
    public byte TargetTerritory;
    
    //Conquer
    public byte ConquerTroopCount;


}