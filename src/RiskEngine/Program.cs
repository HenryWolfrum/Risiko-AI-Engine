using System.Diagnostics;
using RiskEngine;
using RiskEngine.Validation;

Console.WriteLine("========================================");
Console.WriteLine("   RISK ENGINE BENCHMARK & STRESS TEST  ");
Console.WriteLine("========================================");

unsafe
{
    Console.WriteLine($"[MEMORY] GameState Size: {sizeof(GameState)} Bytes");
    Console.WriteLine($"[MEMORY] GameAction Size: {sizeof(GameAction)} Bytes");
}

var game = RiskMapFactory.CreateStandardRiskMap();

// ==========================================
// --- TEST 1: ECHTER KAMPF & EROBERUNG -----
// ==========================================
Console.WriteLine("\n=== 1. SINGLE MATCH TEST ===");

var testPlayers = new IRiskPlayer[game.Config.PlayerCount];
for (var i = 0; i < testPlayers.Length; i++) testPlayers[i] = new SimpleStrategyBot();

var singleMatchSeed = 42;
var testState = GameRunner.PlayGame(game, testPlayers, singleMatchSeed);

Console.WriteLine($"Match beendet nach {testState.CurrentRound} Runden.");
Console.WriteLine(
    $"Verbleibende aktive Spieler: {GameStateHelper.GetActivePlayerCount(in testState)} / {game.Config.PlayerCount}");

// ==========================================
// --- TEST 2: HIGH-SPEED BENCHMARK ---------
// ==========================================
Console.WriteLine("\n=== 2. BENCHMARK: 1.000 MATCHES ===");

var benchmarkMatches = 1_000;

// Warmup
for (var i = 0; i < 10; i++) GameRunner.PlayGame(game, testPlayers, i);

GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

var memoryBefore = GC.GetAllocatedBytesForCurrentThread();
var stopwatch = Stopwatch.StartNew();

for (var i = 0; i < benchmarkMatches; i++) GameRunner.PlayGame(game, testPlayers, i);

stopwatch.Stop();
var memoryAfter = GC.GetAllocatedBytesForCurrentThread();

var totalSeconds = stopwatch.Elapsed.TotalSeconds;
var matchesPerSecond = benchmarkMatches / totalSeconds;
var totalAllocatedBytes = memoryAfter - memoryBefore;

Console.WriteLine("----------------------------------------");
Console.WriteLine($"Gesamtzeit:         {stopwatch.ElapsedMilliseconds} ms ({totalSeconds:F3} s)");
Console.WriteLine($"Durchsatz:          {matchesPerSecond:N0} Matches / Sekunde");
Console.WriteLine($"Allokierte Bytes:   {totalAllocatedBytes} Bytes");
Console.WriteLine("----------------------------------------");

if (totalAllocatedBytes == 0) Console.WriteLine(">>> RESULTAT: PERFECT ZERO ALLOCATION IN GAME LOOP! <<<");

// ==========================================
// --- SIMPLER TEST BOT ---------------------
// ==========================================
public class SimpleStrategyBot : IRiskPlayer
{
    public GameAction DecideAction(in GameState state, GamePhase phase, GameLayout layout)
    {
        var player = state.PlayerTurn;

        switch (phase)
        {
            case GamePhase.Reinforce:
            {
                var target = GameStateHelper.GetFirstTerritoryOwnedBy(in state, player);
                var troops = GameStateHelper.GetPlayerTroopsToPlace(in state, player);

                return new GameAction
                {
                    Type = ActionType.Reinforce,
                    TargetTerritory = target,
                    TroopCount = troops
                };
            }

            case GamePhase.Attack:
            {
                // Greift das erste verfügbare feindliche Nachbargebiet an
                for (byte i = 0; i < EngineConstants.DEFAULT_TERRITORY_COUNT; i++)
                {
                    if (GameStateHelper.GetTerritoryOwner(in state, i) != player) continue;
                    var troops = GameStateHelper.GetTerritoryTroops(in state, i);
                    if (troops <= 1) continue;

                    var neighbors = layout.Map.Adjacencies[i];
                    for (var n = 0; n < neighbors.Length; n++)
                    {
                        var neighborId = neighbors[n];
                        if (GameStateHelper.GetTerritoryOwner(in state, neighborId) != player)
                        {
                            var diceCount = (byte)Math.Min(3, troops - 1);
                            return new GameAction
                            {
                                Type = ActionType.Attack,
                                SourceTerritory = i,
                                TargetTerritory = neighborId,
                                ChosenAttackerDiceCount = diceCount
                            };
                        }
                    }
                }

                return new GameAction { Type = ActionType.SkipPhase };
            }

            case GamePhase.Conquer:
            {
                return new GameAction
                {
                    Type = ActionType.Conquer,
                    ConquerTroopCount = 255
                };
            }

            case GamePhase.Fortify:
            case GamePhase.CardTurnIn:
            default:
                return new GameAction { Type = ActionType.SkipPhase };
        }
    }

    public byte DecideDefenderDice(in GameState state, in GameAction attackAction)
    {
        return AttackRules.GetMaxDefenderDice(in state, attackAction.TargetTerritory);
    }
}