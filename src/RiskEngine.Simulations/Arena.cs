using System;
using System.Diagnostics;
using RiskEngine.AI.Configuration;
using RiskEngine.AI.Factory;
using RiskEngine.State;

namespace RiskEngine.Simulations;

public static class Arena
{
    public static void RunTournament(int numberOfGames, byte playerCount = 4)
    {
        Console.WriteLine($"=== STARTING ARENA TOURNAMENT ({numberOfGames} Games, {playerCount} Players) ===");

        GameLayout layout = RiskMapFactory.CreateStandardRiskMap();

        int[] winCounts = new int[playerCount];
        int totalRounds = 0;
        int maxRoundsReachedCount = 0;

        Stopwatch sw = Stopwatch.StartNew();

        for (int game = 0; game < numberOfGames; game++)
        {
            int gameSeed = game + 133799;

            IRiskPlayer[] players = new IRiskPlayer[playerCount];

            for (byte player = 0; player < playerCount; player++)
            {
                PlayerConfiguration configuration = new RandomBotConfiguration
                {
                    Seed = gameSeed + player
                };

                players[player] = PlayerFactory.Create(configuration);
            }

            GameState state = GameRunner.PlayGame(layout, players, gameSeed);

            if (state.WinnerId != EngineConstants.NO_VALUE)
                winCounts[state.WinnerId]++;
            else
                maxRoundsReachedCount++;

            totalRounds += state.CurrentRound;
        }

        sw.Stop();

        Console.WriteLine("\n=== TOURNAMENT RESULTS ===");
        Console.WriteLine($"Total Time Elapsed: {sw.ElapsedMilliseconds} ms");
        Console.WriteLine($"Games per Second:   {numberOfGames / (sw.ElapsedMilliseconds / 1000.0):F2}");
        Console.WriteLine($"Avg Rounds / Game:  {(double)totalRounds / numberOfGames:F1}");
        Console.WriteLine($"Unfinished Games:   {maxRoundsReachedCount}");
        Console.WriteLine("-----------------------------------");

        for (int player = 0; player < playerCount; player++)
        {
            double winRate = (double)winCounts[player] / numberOfGames * 100.0;
            Console.WriteLine($"Player {player} Winrate: {winRate:F2}% ({winCounts[player]} wins)");
        }
    }
}