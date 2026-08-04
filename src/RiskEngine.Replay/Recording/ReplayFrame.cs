using RiskEngine.State;

namespace RiskEngine.Replay;

public enum ReplayFrameKind : byte
{
    InitialState,
    Action,
    FinalState
}

public struct ReplayFrame
{
    public GameState State;
    public GameAction? Action;
    public ReplayFrameKind Kind;
}