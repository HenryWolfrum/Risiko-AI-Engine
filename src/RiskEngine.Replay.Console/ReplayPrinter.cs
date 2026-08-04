using RiskEngine.Replay;
namespace RiskEngine.Replay.Console;

/// <summary>
/// Prints replay information to the console.
/// </summary>
public static class ReplayPrinter
{
    public static void Print(ReplayPlayer player)
    {
        var frame = player.CurrentFrame;

        System.Console.Clear();

        System.Console.WriteLine("=================================");
        System.Console.WriteLine($"Frame: {player.CurrentFrameIndex}");
        System.Console.WriteLine();

        System.Console.WriteLine($"Round:  {frame.State.CurrentRound}");
        System.Console.WriteLine($"Player: {frame.State.PlayerTurn}");
        System.Console.WriteLine($"Phase:  {frame.State.CurrentPhase}");

        System.Console.WriteLine();

        if (frame.Action.HasValue)
        {
            System.Console.WriteLine($"Action: {frame.Action.Value.Type}");
        }
        else
        {
            System.Console.WriteLine("Action: Initial State");
        }

        System.Console.WriteLine("=================================");
    }
}