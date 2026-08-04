using RiskEngine.AI.Bots;
using RiskEngine.Replay.Recording;
using RiskEngine.State;

namespace RiskEngine.Replay.Console;

internal static class Program
{
    public static void Main()
    {
        int seed = ReadSeed();

        GameLayout layout = RiskMapFactory.CreateStandardRiskMap();

        const byte playerCount = EngineConstants.DEFAULT_PLAYERS;

        var rng = new EngineRandom(seed);

        IRiskPlayer[] players = new IRiskPlayer[playerCount];

        for (byte p = 0; p < playerCount; p++)
        {
            players[p] = new RandomBot(rng);
        }

        System.Console.WriteLine("Players created.");
        
        
        ReplayHeader header = new()
        {
            Seed = seed,
            Layout = layout,
            players = players
            
        };
        
        ReplayRecorder recorder = new(header);
        
        
        GameRunner.PlayGame(layout, players, seed, recorder);
        
        Replay replay = recorder.Build();

        ReplayPlayer player = new(replay);

        ReplayConsole.Run(player);
    }


    private static int ReadSeed()
    {
        System.Console.Write("Seed: ");

        int seed;

        while (!int.TryParse(System.Console.ReadLine(), out seed))
        {
            System.Console.Write("Please enter a valid integer: ");
        }

        return seed;
    }
}