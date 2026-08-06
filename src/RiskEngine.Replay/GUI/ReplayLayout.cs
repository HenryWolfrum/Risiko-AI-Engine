using Raylib_cs;

namespace RiskEngine.Replay.GUI;

public readonly struct ReplayLayout
{
    public Rectangle TopBar { get; init; }

    public Rectangle Map { get; init; }

    public Rectangle PlayerViewer { get; init; }
}