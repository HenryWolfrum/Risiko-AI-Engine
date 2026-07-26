namespace RiskEngine;

public enum GamePhase : byte
{
    Default=0,
    CardTurnIn = 1,
    Reinforce = 2, 
    Attack = 3,
    Conquer = 4,
    Fortify = 5,
    End = 6,
    
    
    Error = byte.MaxValue
}