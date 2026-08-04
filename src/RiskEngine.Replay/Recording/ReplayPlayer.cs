namespace RiskEngine.Replay;

/// <summary>
/// Provides navigation through a recorded replay.
/// </summary>
public sealed class ReplayPlayer
{
    private readonly Replay _replay;

    private int _currentFrame;

    /// <summary>
    /// Creates a replay player.
    /// </summary>
    public ReplayPlayer(Replay replay)
    {
        _replay = replay;
    }

    /// <summary>
    /// Gets the currently selected frame.
    /// </summary>
    public ReplayFrame CurrentFrame => _replay.Frames[_currentFrame];

    /// <summary>
    /// Moves to the next recorded event.
    /// </summary>
    public bool NextEvent()
    {
        if (_currentFrame >= _replay.Frames.Count - 1)
            return false;

        _currentFrame++;
        return true;
    }

    /// <summary>
    /// Moves to the previous recorded event.
    /// </summary>
    public bool PreviousEvent()
    {
        if (_currentFrame == 0)
            return false;

        _currentFrame--;
        return true;
    }

    /// <summary>
    /// Jumps to the specified frame.
    /// </summary>
    public bool JumpTo(int frame)
    {
        if (frame < 0 || frame >= _replay.Frames.Count)
            return false;

        _currentFrame = frame;
        return true;
    }
}