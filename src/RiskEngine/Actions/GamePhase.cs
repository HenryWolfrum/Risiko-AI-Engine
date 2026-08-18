namespace RiskEngine.State;

public enum GamePhase : byte
{
    Start = 0,
    CardTurnIn = 1,
    Reinforce = 2,
    Attack = 3,
    Defend = 4,
    Conquer = 5,
    Fortify = 6,
    Terminated = 7, //Game is decided


    Error = byte.MaxValue
}
