using RiskEngine.State;

namespace RiskEngine.Replay.Recording;

/// <summary>
/// Records simulation frames and builds a replay.
/// </summary>
public sealed class ReplayRecorder : IReplayObserver
{
    private readonly Replay _replay;

    /// <summary>
    /// Creates a new replay recorder.
    /// </summary>
    public ReplayRecorder(ReplayHeader header)
    {
        _replay = new Replay
        {
            Header = header
        };
    }

    /// <inheritdoc />
    public void Record(
        in GameState state,
        in GameAction action)
    {
        _replay.Frames.Add(new ReplayFrame
        {
            State = state,
            Action = action
        });
    }

    /// <summary>
    /// Returns the recorded replay.
    /// </summary>
    public Replay Build()
    {
        return _replay;
    }
}