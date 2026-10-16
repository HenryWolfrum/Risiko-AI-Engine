using RiskEngine.Cards;

namespace RiskEngine.Replay;

public readonly struct ReplayFrame
{
    public required GameState State { get; init; }

    public GameAction? Action { get; init; }
}