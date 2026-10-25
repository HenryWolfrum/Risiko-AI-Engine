using RiskEngine.Actions;
using RiskEngine.State;

namespace RiskEngine.Replay;

/// <summary>
///     Provides navigation through a recorded replay.
/// </summary>
public sealed class ReplayPlayer
{
    public readonly Replay _replay;

    /// <summary>
    ///     Creates a replay player.
    /// </summary>
    public ReplayPlayer(Replay replay)
    {
        ArgumentNullException.ThrowIfNull(replay);

        if (replay.Frames.Count == 0)
            throw new ArgumentException("Replay contains no frames.");

        _replay = replay;
    }

    /// <summary>
    ///     Gets the currently selected frame.
    /// </summary>
    public ReplayFrame CurrentFrame => _replay.Frames[CurrentFrameIndex];

    /// <summary>
    ///     Gets the index of the currently selected frame.
    /// </summary>
    public int CurrentFrameIndex { get; private set; }

    /// <summary>
    ///     Gets the total number of recorded frames.
    /// </summary>
    public int FrameCount => _replay.Frames.Count;

    /// <summary>
    ///     Gets whether the current frame is the first frame.
    /// </summary>
    public bool IsFirstFrame => CurrentFrameIndex == 0;

    /// <summary>
    ///     Gets whether the current frame is the last frame.
    /// </summary>
    public bool IsLastFrame => CurrentFrameIndex == _replay.Frames.Count - 1;

    /// <summary>
    ///     Moves to the next recorded event.
    /// </summary>
    public bool NextFrame()
    {
        if (IsLastFrame)
            return false;

        CurrentFrameIndex++;
        return true;
    }

    /// <summary>
    ///     Moves to the previous recorded event.
    /// </summary>
    public bool PreviousFrame()
    {
        if (IsFirstFrame)
            return false;

        CurrentFrameIndex--;
        return true;
    }

    /// <summary>
    ///     Moves to the first frame of the next round.
    /// </summary>
    public bool NextRound()
    {
        ushort currentRound = CurrentFrame.State.CurrentRound;

        while (NextFrame())
            if (CurrentFrame.State.CurrentRound != currentRound)
                return true;

        return false;
    }


    /// <summary>
    ///     Moves to the first frame of the next player's turn.
    /// </summary>
    public bool NextPlayer()
    {
        byte currentPlayer = GameStateHelper.GetPlayerOnTurn(CurrentFrame.State);

        while (NextFrame())
            if (GameStateHelper.GetPlayerOnTurn(CurrentFrame.State) != currentPlayer)
                return true;

        return false;
    }


    public bool NextPhase()
    {
        GamePhase currentPhase = CurrentFrame.State.CurrentPhase;

        while (NextFrame())
            if (CurrentFrame.State.CurrentPhase != currentPhase)
                return true;

        return false;
    }

    /// <summary>
    ///     Moves to the first frame of the previous round.
    /// </summary>
    public bool PreviousRound()
    {
        if (IsFirstFrame) return false;

        ushort startRound = CurrentFrame.State.CurrentRound;

        // 1. Zurückgehen, bis wir nicht mehr in der aktuellen Runde sind
        while (!IsFirstFrame && CurrentFrame.State.CurrentRound == startRound) PreviousFrame();

        // 2. Falls wir in einer früheren Runde gelandet sind, bis zu deren ERSTEM Frame zurückspulen
        if (CurrentFrame.State.CurrentRound != startRound)
        {
            ushort targetRound = CurrentFrame.State.CurrentRound;
            while (CurrentFrameIndex > 0 && _replay.Frames[CurrentFrameIndex - 1].State.CurrentRound == targetRound)
                CurrentFrameIndex--;
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Moves to the first frame of the previous player's turn.
    /// </summary>
    public bool PreviousPlayer()
    {
        if (IsFirstFrame) return false;

        byte startPlayer = GameStateHelper.GetPlayerOnTurn(CurrentFrame.State);

        while (!IsFirstFrame && GameStateHelper.GetPlayerOnTurn(CurrentFrame.State) == startPlayer) PreviousFrame();

        if (GameStateHelper.GetPlayerOnTurn(CurrentFrame.State) != startPlayer)
        {
            byte targetPlayer = GameStateHelper.GetPlayerOnTurn(CurrentFrame.State);
            while (CurrentFrameIndex > 0 &&GameStateHelper.GetPlayerOnTurn( _replay.Frames[CurrentFrameIndex - 1].State)== targetPlayer)
                CurrentFrameIndex--;
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Moves to the first frame of the previous phase.
    /// </summary>
    public bool PreviousPhase()
    {
        if (IsFirstFrame) return false;

        GamePhase startPhase = CurrentFrame.State.CurrentPhase;

        while (!IsFirstFrame && CurrentFrame.State.CurrentPhase == startPhase) PreviousFrame();

        if (CurrentFrame.State.CurrentPhase != startPhase)
        {
            GamePhase targetPhase = CurrentFrame.State.CurrentPhase;
            while (CurrentFrameIndex > 0 && _replay.Frames[CurrentFrameIndex - 1].State.CurrentPhase == targetPhase)
                CurrentFrameIndex--;
            return true;
        }

        return false;
    }


    /// <summary>
    ///     Jumps to the specified frame.
    /// </summary>
    public bool JumpTo(int frame)
    {
        if (frame < 0 || frame >= FrameCount)
            return false;

        CurrentFrameIndex = frame;
        return true;
    }

    /// <summary>
    ///     Gets the frame at the specified index without changing the current frame.
    /// </summary>
    public ReplayFrame GetFrame(int index)
    {
        return _replay.Frames[index];
    }
}