using RiskEngine.Actions;
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

    public void OnGameStarted(in GameState state)
    {
        AddFrame(in state, null);
    }

    public void OnActionApplied(in GameState state, in GameAction action)
    {
        AddFrame(in state, action);
    }

    public void OnGameFinished(in GameState state)
    {
        AddFrame(in state, null);
    }

    private void AddFrame(in GameState state, GameAction? action)
    {
        _replay.Frames.Add(new ReplayFrame
        {
            State = state,
            Action = action
        });
    }

    public Replay Build()
    {
        return _replay;
    }
}