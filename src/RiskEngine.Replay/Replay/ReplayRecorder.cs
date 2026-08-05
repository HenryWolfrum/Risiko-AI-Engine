using RiskEngine.Observer;
using RiskEngine.State;

namespace RiskEngine.Replay.Recording;

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