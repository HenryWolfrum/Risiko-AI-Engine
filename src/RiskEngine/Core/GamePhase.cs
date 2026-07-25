namespace RiskEngine;

public enum GamePhase : byte
{
    Default=0,
    Reinforce = 1, 
    Attack = 2,
    Fortify = 3,
    End = 4,
    
    
    Error = byte.MaxValue
}