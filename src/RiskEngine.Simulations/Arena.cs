using System;
using System.Diagnostics;
using RiskEngine.AI.Configuration;
using RiskEngine.AI.Factory;
using RiskEngine.State;

namespace RiskEngine.Simulations;

public static class Arena
{
    public static void RunTournament(ulong numberOfGames, byte playerCount = 4)
    {
        Console.WriteLine($"=== STARTING ROTATING ARENA TOURNAMENT ({numberOfGames} Games, {playerCount} Players) ===");

        GameLayout layout = RiskMapFactory.CreateStandardRiskMap();

        // 1. Statistische Tracker
        int aggroWins = 0;
        int randomWins = 0;
        int[] seatWins = new int[playerCount]; // Erfasst Siege pro Slot (0, 1, 2, 3)
        
        int totalRounds = 0;
        int maxRoundsReachedCount = 0;

        Stopwatch sw = Stopwatch.StartNew();

        for (ulong game = 0; game < numberOfGames; game++)
        {
            ulong gameSeed = game + 20000;

            // Der AggroBot wandert in jedem Spiel einen Sitzplatz weiter
            byte aggroSlot = (byte)(game % playerCount);

            IRiskPlayer[] players = new IRiskPlayer[playerCount];

            for (byte slot = 0; slot < playerCount; slot++)
            {
                PlayerConfiguration configuration = (slot == aggroSlot)
                    ? new AggroBotConfiguration()
                    : new RandomBotConfiguration { Seed = gameSeed + slot };

                players[slot] = PlayerFactory.Create(configuration);
            }

            GameState state = GameRunner.PlayGame(layout, players, gameSeed);

            if (state.WinnerId != EngineConstants.NO_VALUE)
            {
                // Erfasse Gewinn nach Sitzplatz
                seatWins[state.WinnerId]++;

                // Erfasse Gewinn nach Bot-Typ
                if (state.WinnerId == aggroSlot)
                    aggroWins++;
                else
                    randomWins++;
            }
            else
            {
                maxRoundsReachedCount++;
            }

            totalRounds += state.CurrentRound;
        }

        sw.Stop();

        double totalFinishedGames = numberOfGames - (ulong)maxRoundsReachedCount;

        Console.WriteLine("\n=== TOURNAMENT RESULTS ===");
        Console.WriteLine($"Total Time Elapsed: {sw.ElapsedMilliseconds} ms");
        Console.WriteLine($"Games per Second:   {numberOfGames / (sw.ElapsedMilliseconds / 1000.0):F2}");
        Console.WriteLine($"Avg Rounds / Game:  {(double)totalRounds / numberOfGames:F1}");
        Console.WriteLine($"Unfinished Games:   {maxRoundsReachedCount}");
        
        Console.WriteLine("\n--- BOT PERFORMANCE (Position-Agnostic) ---");
        double aggroWinrate = (double)aggroWins / numberOfGames * 100.0;
        double randomWinrate = (double)randomWins / numberOfGames * 100.0;
        
        Console.WriteLine($"AggroBot Overall Winrate:  {aggroWinrate,6:F2}% ({aggroWins} wins)");
        Console.WriteLine($"RandomBots Combined Winrate: {randomWinrate,6:F2}% ({randomWins} wins)");

        Console.WriteLine("\n--- SEAT BIAS (Wins per Slot) ---");
        for (int slot = 0; slot < playerCount; slot++)
        {
            double seatWinrate = (double)seatWins[slot] / numberOfGames * 100.0;
            Console.WriteLine($"Slot {slot} Winrate: {seatWinrate,6:F2}% ({seatWins[slot]} wins)");
        }
    }
}