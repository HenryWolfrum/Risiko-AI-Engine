using System.Diagnostics;
using RiskEngine.AI.Configuration;
using RiskEngine.AI.Factory;
using RiskEngine.Constants;
using RiskEngine.Core;
using RiskEngine.Layout;
using RiskEngine.Player;

namespace RiskEngine.Simulations;

public static class Arena
{
    public static void Run(
        ulong gamesPerScenario = 10_000,
        byte playerCount = 4,
        ulong experimentSeed = 20_000)
    {
        if (playerCount < EngineConstants.MIN_PLAYERS ||
            playerCount > EngineConstants.MAX_PLAYERS)
        {
            throw new ArgumentOutOfRangeException(nameof(playerCount));
        }

        if (gamesPerScenario == 0 ||
            gamesPerScenario % playerCount != 0)
        {
            throw new ArgumentException(
                "Game count must be positive and divisible by player count.",
                nameof(gamesPerScenario));
        }

        var layout = RiskMapFactory.CreateStandardRiskMap();

        Console.WriteLine("=== CONTROLLED AGGRO EXPERIMENT ===");
        Console.WriteLine($"Games per scenario: {gamesPerScenario}");
        Console.WriteLine($"Players:            {playerCount}");
        Console.WriteLine($"Experiment seed:    {experimentSeed}");
        Console.WriteLine("Seed blocks rotate through every seat.");

        RunScenario(
            layout, gamesPerScenario, playerCount,
            experimentSeed, useAggro: true);

        RunScenario(
            layout, gamesPerScenario, playerCount,
            experimentSeed, useAggro: false);
    }

    private static void RunScenario(
        GameLayout layout,
        ulong games,
        byte playerCount,
        ulong experimentSeed,
        bool useAggro)
    {
        ulong aggroWins = 0;
        ulong randomWins = 0;
        ulong stalemates = 0;
        long totalRounds = 0;
        var seatWins = new ulong[playerCount];

        var stopwatch = Stopwatch.StartNew();

        for (ulong game = 0; game < games; game++)
        {
            // Each block reuses one game seed across all seat rotations.
            ulong block = game / playerCount;
            byte rotation = (byte)(game % playerCount);
            ulong gameSeed = experimentSeed + block;

            // In the mixed scenario, rotate AggroBot through all slots.
            byte aggroSlot = rotation;

            var players = new IRiskPlayer[playerCount];

            for (byte slot = 0; slot < playerCount; slot++)
            {
                bool isAggro = useAggro && slot == aggroSlot;

                PlayerConfiguration config = isAggro
                    ? new AggroBotConfiguration()
                    : new RandomBotConfiguration
                    {
                        // Independent, reproducible bot RNG per game and slot.
                        Seed = gameSeed
                            + 1_000_000UL
                            + game * playerCount
                            + slot
                    };

                players[slot] = PlayerFactory.Create(config);
            }

            var state = GameRunner.PlayGame(
                layout, players, gameSeed);

            totalRounds += state.CurrentRound;

            if (state.WinnerId == EngineConstants.NO_VALUE)
            {
                stalemates++;
                continue;
            }

            seatWins[state.WinnerId]++;

            if (useAggro && state.WinnerId == aggroSlot)
                aggroWins++;
            else
                randomWins++;
        }

        stopwatch.Stop();

        ulong decisiveGames = games - stalemates;

        Console.WriteLine();
        Console.WriteLine(useAggro
            ? "--- 1 AGGROBOT + RANDOM BOTS ---"
            : "--- ALL RANDOM BOTS (CONTROL) ---");

        Console.WriteLine($"Elapsed:        {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Games/sec:      {games / stopwatch.Elapsed.TotalSeconds:F2}");
        Console.WriteLine($"Average rounds: {(double)totalRounds / games:F1}");
        Console.WriteLine($"Stalemates:     {stalemates} ({100.0 * stalemates / games:F2}%)");
        Console.WriteLine($"Decisive games: {decisiveGames}");

        Console.WriteLine(
            $"Aggro wins:     {aggroWins} ({100.0 * aggroWins / games:F2}%)");

        Console.WriteLine(
            $"Random wins:    {randomWins} ({100.0 * randomWins / games:F2}%)");

        if (decisiveGames > 0 && useAggro)
        {
            Console.WriteLine(
                $"Aggro win share among decisive games: " +
                $"{100.0 * aggroWins / decisiveGames:F2}%");
        }

        Console.WriteLine("Seat wins:");

        for (byte slot = 0; slot < playerCount; slot++)
        {
            Console.WriteLine(
                $"  Slot {slot}: {seatWins[slot]} " +
                $"({100.0 * seatWins[slot] / games:F2}%)");
        }
    }
}