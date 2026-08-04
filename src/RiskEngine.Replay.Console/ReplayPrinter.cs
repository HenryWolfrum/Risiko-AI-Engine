using RiskEngine.Replay;
using RiskEngine.State;

namespace RiskEngine.Replay.Console;

public static class ReplayPrinter
{
    public static void Print(ReplayPlayer player, GameLayout layout)
    {
        var frame = player.CurrentFrame;
        var state = frame.State;

        System.Console.Clear();
        System.Console.WriteLine("==================================================");
        System.Console.WriteLine($"FRAME {player.CurrentFrameIndex} / {player.FrameCount - 1}");
        System.Console.WriteLine();

        System.Console.WriteLine($"Round : {state.CurrentRound}");
        System.Console.WriteLine($"Player: {state.PlayerTurn}");
        System.Console.WriteLine($"Phase : {state.CurrentPhase}");

        System.Console.WriteLine();
        System.Console.WriteLine("ACTION");
        System.Console.WriteLine("----------------------------------");

        PrintAction(frame, layout);

        System.Console.WriteLine("==================================================");
    }

    private static void PrintAction(ReplayFrame frame, GameLayout layout)
    {
        if (frame.Kind == ReplayFrameKind.InitialState)
        {
            System.Console.WriteLine("Initial Game State");
            return;
        }

        if (frame.Kind == ReplayFrameKind.FinalState)
        {
            System.Console.WriteLine($"Game Finished - Winner: Player {frame.State.WinnerId}");
            return;
        }

        if (!frame.Action.HasValue)
        {
            System.Console.WriteLine("None");
            return;
        }

        var value = frame.Action.Value;
        var state = frame.State;

        switch (value.Type)
        {
            case ActionType.Attack:
                System.Console.WriteLine("Attack");
                PrintTerritory("From", state, layout, value.SourceTerritory);
                PrintTerritory("To", state, layout, value.TargetTerritory);
                System.Console.WriteLine();
                System.Console.WriteLine($"Dice: {value.ChosenAttackerDiceCount} vs {value.ChosenDefenderDiceCount}");
                break;

            case ActionType.Conquer:
                System.Console.WriteLine("Conquer");
                PrintTerritory("From", state, layout, value.SourceTerritory);
                PrintTerritory("To", state, layout, value.TargetTerritory);
                System.Console.WriteLine($"Moving troops: {value.TroopCount}");
                break;

            case ActionType.Reinforce:
                System.Console.WriteLine("Reinforce");
                PrintTerritory("Target", state, layout, value.TargetTerritory);
                System.Console.WriteLine($"Placing troops: {value.TroopCount}");
                break;

            case ActionType.Fortify:
                System.Console.WriteLine("Fortify");
                PrintTerritory("From", state, layout, value.SourceTerritory);
                PrintTerritory("To", state, layout, value.TargetTerritory);
                System.Console.WriteLine($"Moving troops: {value.TroopCount}");
                break;

            case ActionType.TurnInCards:
                System.Console.WriteLine("Turn in cards");
                break;
        }
    }

    private static void PrintTerritory(string label, GameState state, GameLayout layout, byte territory)
    {
        string name = layout.Map.TerritoryNames[territory];
        byte troops = GameStateHelper.GetTerritoryTroops(state, territory);
        byte owner = GameStateHelper.GetTerritoryOwner(state, territory);

        System.Console.WriteLine($"{label}: {name}");
        System.Console.WriteLine($"       Owner : Player {owner}");
        System.Console.WriteLine($"       Troops: {troops}");
    }
}