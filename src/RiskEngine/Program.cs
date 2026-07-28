using System;
using System.Diagnostics;
using RiskEngine;
using RiskEngine.Rules;
using RiskEngine.Validation;

Console.WriteLine("========================================");
Console.WriteLine("   RISK ENGINE TEST & BENCHMARK");
Console.WriteLine("========================================");


unsafe
{
    Console.WriteLine($"[MEMORY] GameState Size : {sizeof(GameState)} Bytes");
    Console.WriteLine($"[MEMORY] GameAction Size: {sizeof(GameAction)} Bytes");
}


var game = RiskMapFactory.CreateStandardRiskMap();

var players = new IRiskPlayer[game.Config.PlayerCount];

for (var i = 0; i < players.Length; i++)
{
    players[i] = new SimpleStrategyBot();
}


// ==========================================
// TEST 1: SINGLE GAME
// ==========================================

Console.WriteLine();
Console.WriteLine("=== 1. SINGLE GAME TEST ===");


var state = GameRunner.PlayGame(
    game,
    players,
    seed: 42);


Console.WriteLine($"Finished after round : {state.CurrentRound}");
Console.WriteLine(
    $"Active players      : {GameStateHelper.GetActivePlayerCount(in state)} / {game.Config.PlayerCount}");


// ==========================================
// TEST 2: DETERMINISM TEST
// ==========================================

Console.WriteLine();
Console.WriteLine("=== 2. DETERMINISM TEST ===");


var firstRun = GameRunner.PlayGame(game, players, 123);
var secondRun = GameRunner.PlayGame(game, players, 123);


Console.WriteLine(
    firstRun.CurrentRound == secondRun.CurrentRound
        ? "PASS: Same seed creates same result"
        : "FAIL: Randomness is not deterministic");


// ==========================================
// TEST 3: RANDOM SEED STABILITY
// ==========================================

Console.WriteLine();
Console.WriteLine("=== 3. RANDOM SEED TEST ===");


for (var seed = 0; seed < 20; seed++)
{
    var result = GameRunner.PlayGame(game, players, seed);

    Console.WriteLine(
        $"Seed {seed,2}: Round {result.CurrentRound,4} | Alive {GameStateHelper.GetActivePlayerCount(in result)}");
}


// ==========================================
// TEST 4: BENCHMARK
// ==========================================

Console.WriteLine();
Console.WriteLine("=== 4. BENCHMARK 1000 MATCHES ===");


const int matches = 1000;


// Warmup
for (var i = 0; i < 20; i++)
{
    GameRunner.PlayGame(game, players, i);
}


GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();


var memoryBefore = GC.GetAllocatedBytesForCurrentThread();

var stopwatch = Stopwatch.StartNew();


for (var i = 0; i < matches; i++)
{
    GameRunner.PlayGame(game, players, i);
}


stopwatch.Stop();


var memoryAfter = GC.GetAllocatedBytesForCurrentThread();


var seconds = stopwatch.Elapsed.TotalSeconds;


Console.WriteLine("----------------------------------------");
Console.WriteLine($"Time:        {stopwatch.ElapsedMilliseconds} ms");
Console.WriteLine($"Speed:       {matches / seconds:N0} games/sec");
Console.WriteLine($"Allocation:  {memoryAfter - memoryBefore:N0} bytes");
Console.WriteLine("----------------------------------------");



// ==========================================
// SIMPLE TEST BOT
// ==========================================

public class SimpleStrategyBot : IRiskPlayer
{
    public GameAction DecideAction(
        in GameState state,
        GamePhase phase,
        GameLayout layout)
    {
        var player = state.PlayerTurn;


        switch (phase)
        {
            case GamePhase.Reinforce:
            {
                var territory =
                    GameStateHelper.GetFirstTerritoryOwnedBy(
                        in state,
                        player);


                return new GameAction
                {
                    Type = ActionType.Reinforce,
                    TargetTerritory = territory,
                    TroopCount =
                        GameStateHelper.GetPlayerTroopsToPlace(
                            in state,
                            player)
                };
            }


            case GamePhase.Attack:
            {
                for (byte territory = 0;
                     territory < EngineConstants.MAX_TERRITORIES;
                     territory++)
                {
                    if (GameStateHelper.GetTerritoryOwner(in state, territory) != player)
                        continue;


                    var troops =
                        GameStateHelper.GetTerritoryTroops(
                            in state,
                            territory);


                    if (troops <= 1)
                        continue;


                    foreach (var neighbour in layout.Map.Adjacencies[territory])
                    {
                        if (GameStateHelper.GetTerritoryOwner(in state, neighbour) == player)
                            continue;


                        return new GameAction
                        {
                            Type = ActionType.Attack,
                            SourceTerritory = territory,
                            TargetTerritory = neighbour,
                            ChosenAttackerDiceCount =
                                (byte)Math.Min(3, troops - 1)
                        };
                    }
                }


                return new GameAction
                {
                    Type = ActionType.EndTurn
                };
            }


            case GamePhase.Conquer:
            {
                // Conservative fallback:
                // Move only the minimum required troop.
                return new GameAction
                {
                    Type = ActionType.Conquer,
                    ConquerTroopCount = 1
                };
            }


            case GamePhase.CardTurnIn:
            case GamePhase.Fortify:
            default:
            {
                return new GameAction
                {
                    Type = ActionType.EndTurn
                };
            }
        }
    }



    public byte DecideDefenderDice(
        in GameState state,
        in GameAction attackAction)
    {
        return AttackRules.GetMaxDefenderDice(
            in state,
            attackAction.TargetTerritory);
    }
}