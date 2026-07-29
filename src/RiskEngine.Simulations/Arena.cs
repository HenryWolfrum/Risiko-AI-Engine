using System;
using System.Diagnostics;
using RiskEngine.AI.Bots;
using RiskEngine.State;

namespace RiskEngine.Simulations;

public static class Arena
{
    public static void RunTournament(int numberOfGames, byte playerCount = 4)
    {
        Console.WriteLine($"=== STARTING ARENA TOURNAMENT ({numberOfGames} Games, {playerCount} Players) ===");
        
        var layout = RiskMapFactory.CreateStandardRiskMap();
        int[] winCounts = new int[playerCount];
        int totalRounds = 0;
        int maxRoundsReachedCount = 0;

        Stopwatch sw = Stopwatch.StartNew();

        for (int i = 0; i < numberOfGames; i++)
        {
     
            var rng = new EngineRandom((i + 133799));

            IRiskPlayer[] players = new IRiskPlayer[playerCount];
            for (byte p = 0; p < playerCount; p++)
            {
                players[p] = new RandomBot(rng);
            }

            GameState state = GameRunner.PlayGame(layout, players, i + 133799);

            if (state.WinnerId != EngineConstants.NO_VALUE)
            {
                winCounts[state.WinnerId]++;
            }
            else
            {
                maxRoundsReachedCount++;
            }

            totalRounds += state.CurrentRound;
        }

        sw.Stop();

        Console.WriteLine("\n=== TOURNAMENT RESULTS ===");
        Console.WriteLine($"Total Time Elapsed: {sw.ElapsedMilliseconds} ms");
        Console.WriteLine($"Games per Second:   {numberOfGames / (sw.ElapsedMilliseconds / 1000.0):F2}");
        Console.WriteLine($"Avg Rounds / Game:  {(double)totalRounds / numberOfGames:F1}");
        Console.WriteLine($"Unfinished Games:   {maxRoundsReachedCount}");
        Console.WriteLine("-----------------------------------");
        
        for (int p = 0; p < playerCount; p++)
        {
            double winRate = (double)winCounts[p] / numberOfGames * 100.0;
            Console.WriteLine($"Player {p} Winrate:   {winRate:F2}% ({winCounts[p]} wins)");
        }
    }
}