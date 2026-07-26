namespace RiskEngine;

//Generic GameAction 
public struct GameAction
{
    public ActionType Type;

    //For Card Retrieval
    public byte Card1;
    public byte Card2;
    public byte Card3;

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