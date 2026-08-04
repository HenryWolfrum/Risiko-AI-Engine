using RiskEngine.Observer;
using RiskEngine.State;

namespace RiskEngine.Replay.Recording;

public sealed class ReplayRecorder : IGameObserver
{
    private readonly Replay _replay;

    public ReplayRecorder(ReplayHeader header)
    {
        // Header direkt beim Erstellen des Replay-Objekts zuweisen
        _replay = new Replay
        {
            Header = header
        };
    }

    public void RecordInitialState(in GameState state)
    {
        _replay.Frames.Add(new ReplayFrame
        {
            State = state,
            Action = null,
            Kind = ReplayFrameKind.InitialState
        });
    }

    public void Record(in GameState state, in GameAction action)
    {
        _replay.Frames.Add(new ReplayFrame
        {
            State = state,
            Action = action,
            Kind = ReplayFrameKind.Action
        });
    }

    public void RecordFinalState(in GameState state)
    {
        _replay.Frames.Add(new ReplayFrame
        {
            State = state,
            Action = null,
            Kind = ReplayFrameKind.FinalState
        });
    }

    public Replay Build() => _replay;
}