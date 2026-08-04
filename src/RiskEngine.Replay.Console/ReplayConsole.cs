using RiskEngine.Replay;
using RiskEngine.State;

namespace RiskEngine.Replay.Console;

/// <summary>
/// Provides a simple console interface for navigating through a replay.
/// </summary>
public static class ReplayConsole
{
    public static void Run(ReplayPlayer player, GameLayout layout)
    {
        while (true)
        {
            ReplayPrinter.Print(player, layout);

            System.Console.WriteLine();
            System.Console.WriteLine("Commands:");
            System.Console.WriteLine("  n  / p   - next / previous event");
            System.Console.WriteLine("  np / pp  - next / previous player");
            System.Console.WriteLine("  nr / pr  - next / previous round");
            System.Console.WriteLine("  ph / pph - next / previous phase");
            System.Console.WriteLine("  j <frame>- jump to frame index (e.g. 'j 42')");
            System.Console.WriteLine("  q        - quit");

            System.Console.Write("> ");

            string? input = System.Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(input))
                continue;

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string command = parts[0].ToLowerInvariant();

            switch (command)
            {
                case "n":
                    player.NextEvent();
                    break;

                case "p":
                    player.PreviousEvent();
                    break;

                case "np":
                    player.NextPlayer();
                    break;

                case "pp":
                    player.PreviousPlayer();
                    break;

                case "nr":
                    player.NextRound();
                    break;

                case "pr":
                    player.PreviousRound();
                    break;

                case "ph":
                    player.NextPhase();
                    break;

                case "pph":
                    player.PreviousPhase();
                    break;

                case "j":
                case "jump":
                case "goto":
                    if (parts.Length > 1 && int.TryParse(parts[1], out int targetFrame))
                    {
                        player.JumpTo(targetFrame);
                    }
                    break;

                case "q":
                    return;
            }
        }
    }
}