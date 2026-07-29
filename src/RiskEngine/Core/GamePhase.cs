namespace RiskEngine.State;

public enum GamePhase : byte
{
    Default = 0,
    CardTurnIn = 1,
    Reinforce = 2,
    Attack = 3,
    Conquer = 4,
    Fortify = 5,
    Terminated = 6, //Game is decided


    Error = byte.MaxValue
}