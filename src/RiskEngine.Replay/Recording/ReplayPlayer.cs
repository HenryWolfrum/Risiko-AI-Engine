using RiskEngine.State;

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
        _currentFrame = 0;
    }

    /// <summary>
    /// Gets the currently selected frame.
    /// </summary>
    public ReplayFrame CurrentFrame => _replay.Frames[_currentFrame];

    /// <summary>
    /// Gets the index of the currently selected frame.
    /// </summary>
    public int CurrentFrameIndex => _currentFrame;

    /// <summary>
    /// Gets the total number of recorded frames.
    /// </summary>
    public int FrameCount => _replay.Frames.Count;

    /// <summary>
    /// Gets whether the current frame is the first frame.
    /// </summary>
    public bool IsFirstFrame => _currentFrame == 0;

    /// <summary>
    /// Gets whether the current frame is the last frame.
    /// </summary>
    public bool IsLastFrame => _currentFrame == _replay.Frames.Count - 1;

    /// <summary>
    /// Moves to the next recorded event.
    /// </summary>
    public bool NextEvent()
    {
        if (IsLastFrame)
            return false;

        _currentFrame++;
        return true;
    }

    /// <summary>
    /// Moves to the previous recorded event.
    /// </summary>
    public bool PreviousEvent()
    {
        if (IsFirstFrame)
            return false;

        _currentFrame--;
        return true;
    }

    /// <summary>
    /// Moves to the first frame of the next round.
    /// </summary>
    public bool NextRound()
    {
        ushort currentRound = CurrentFrame.State.CurrentRound;

        while (NextEvent())
        {
            if (CurrentFrame.State.CurrentRound != currentRound)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Moves to the first frame of the previous round.
    /// </summary>
    public bool PreviousRound()
    {
        ushort currentRound = CurrentFrame.State.CurrentRound;

        while (PreviousEvent())
        {
            if (CurrentFrame.State.CurrentRound != currentRound)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Moves to the first frame of the next player's turn.
    /// </summary>
    public bool NextPlayer()
    {
        byte currentPlayer = CurrentFrame.State.PlayerTurn;

        while (NextEvent())
        {
            if (CurrentFrame.State.PlayerTurn != currentPlayer)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Moves to the first frame of the previous player's turn.
    /// </summary>
    public bool PreviousPlayer()
    {
        byte currentPlayer = CurrentFrame.State.PlayerTurn;

        while (PreviousEvent())
        {
            if (CurrentFrame.State.PlayerTurn != currentPlayer)
                return true;
        }

        return false;
    }

    public bool NextPhase()
    {
        GamePhase currentPhase = CurrentFrame.State.CurrentPhase;

        while (NextEvent())
        {
            if (CurrentFrame.State.CurrentPhase != currentPhase)
                return true;
        }

        return false;
    }

    public bool PreviousPhase()
    {
        GamePhase currentPhase = CurrentFrame.State.CurrentPhase;

        while (PreviousEvent())
        {
            if (CurrentFrame.State.CurrentPhase != currentPhase)
            {
                return true;
            }
        }
        return false;
    }
    
    
    
    /// <summary>
    /// Jumps to the specified frame.
    /// </summary>
    public bool JumpTo(int frame)
    {
        if (frame < 0 || frame >= FrameCount)
            return false;

        _currentFrame = frame;
        return true;
    }
}