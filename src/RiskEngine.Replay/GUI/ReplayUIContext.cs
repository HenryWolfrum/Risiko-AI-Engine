namespace RiskEngine.Replay.GUI;

public class ReplayUIContext
{
    public ReplayPlayer Player { get; }
    public ReplayMode Mode { get; }
    public ReplayAction PendingAction { get; set; } = ReplayAction.None;

    public ReplayUIContext(ReplayPlayer player, ReplayMode mode)
    {
        Player = player;
        Mode = mode;
    }
}