using RiskEngine.Replay;
using RiskEngine.State;

namespace RiskEngine.Replay.Console;

/// <summary>
/// Provides a simple console interface for navigating through a replay.
/// </summary>
public static class ReplayConsole
{
    public static void Run(ReplayPlayer player,GameLayout layout)
    {
        while (true)
        {
            ReplayPrinter.Print(player,layout);

            System.Console.WriteLine();
            System.Console.WriteLine("Commands:");
            System.Console.WriteLine("n - next event");
            System.Console.WriteLine("p - previous event");
            System.Console.WriteLine("q - quit");

            System.Console.Write("> ");

            string? command = System.Console.ReadLine();

            switch (command)
            {
                case "n":
                    player.NextEvent();
                    break;

                case "p":
                    player.PreviousEvent();
                    break;

                case "q":
                    return;
            }
        }
    }
}