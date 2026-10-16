using RiskEngine.Layout.Observer;
using RiskEngine.Layout;

namespace RiskEngine.Layout.Replay.Recording;

public sealed class ReplayRecorder : IGameObserver
{
    private readonly Replay _replay;

    public ReplayRecorder(ReplayHeader header)
    {
        _replay = new Replay
        {
            Header = header
        };
    }


    public void Record(in GameState state, GameAction? action)
    {
        _replay.Frames.Add(new ReplayFrame
        {
            State = state,
            Action = action
        });
    }


    public Replay Build() => _replay;
}