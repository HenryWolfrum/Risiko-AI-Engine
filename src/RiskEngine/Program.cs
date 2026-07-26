using System;
using System.Diagnostics;
using RiskEngine;
using RiskEngine.Validation;
using RiskEngine.Resolution;

Console.WriteLine("========================================");
Console.WriteLine("   RISK ENGINE BENCHMARK & STRESS TEST  ");
Console.WriteLine("========================================");

unsafe
{
    Console.WriteLine($"[MEMORY] GameState Size: {sizeof(GameState)} Bytes");
    Console.WriteLine($"[MEMORY] GameAction Size: {sizeof(GameAction)} Bytes");
}

GameLayout game = RiskMapFactory.CreateStandardRiskMap();

// ==========================================
// --- TEST 1: ECHTER KAMPF & EROBERUNG -----
// ==========================================
Console.WriteLine("\n=== 1. SINGLE MATCH TEST ===");

IRiskPlayer[] testPlayers = new IRiskPlayer[game.Config.PlayerCount];
for (int i = 0; i < testPlayers.Length; i++)
{
    testPlayers[i] = new SimpleStrategyBot();
}

int singleMatchSeed = 42;
GameState testState = GameRunner.PlayGame(game, testPlayers, singleMatchSeed);

Console.WriteLine($"Match beendet nach {testState.CurrentRound} Runden.");
Console.WriteLine($"Verbleibende aktive Spieler: {GameStateHelper.GetActivePlayerCount(in testState)} / {game.Config.PlayerCount}");

// ==========================================
// --- TEST 2: HIGH-SPEED BENCHMARK ---------
// ==========================================
Console.WriteLine("\n=== 2. BENCHMARK: 1.000 MATCHES ===");

int benchmarkMatches = 1_000;

// Warmup
for (int i = 0; i < 10; i++)
{
    GameRunner.PlayGame(game, testPlayers, i);
}

GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

long memoryBefore = GC.GetAllocatedBytesForCurrentThread();
Stopwatch stopwatch = Stopwatch.StartNew();

for (int i = 0; i < benchmarkMatches; i++)
{
    GameRunner.PlayGame(game, testPlayers, i);
}

stopwatch.Stop();
long memoryAfter = GC.GetAllocatedBytesForCurrentThread();

double totalSeconds = stopwatch.Elapsed.TotalSeconds;
double matchesPerSecond = benchmarkMatches / totalSeconds;
long totalAllocatedBytes = memoryAfter - memoryBefore;

Console.WriteLine($"----------------------------------------");
Console.WriteLine($"Gesamtzeit:         {stopwatch.ElapsedMilliseconds} ms ({totalSeconds:F3} s)");
Console.WriteLine($"Durchsatz:          {matchesPerSecond:N0} Matches / Sekunde");
Console.WriteLine($"Allokierte Bytes:   {totalAllocatedBytes} Bytes");
Console.WriteLine($"----------------------------------------");

if (totalAllocatedBytes == 0)
{
    Console.WriteLine(">>> RESULTAT: PERFECT ZERO ALLOCATION IN GAME LOOP! <<<");
}

// ==========================================
// --- SIMPLER TEST BOT ---------------------
// ==========================================
public class SimpleStrategyBot : IRiskPlayer
{
    public GameAction DecideAction(in GameState state, GamePhase phase, GameLayout layout)
    {
        byte player = state.PlayerTurn;

        switch (phase)
        {
            case GamePhase.Reinforce:
            {
                byte target = GameStateHelper.GetFirstTerritoryOwnedBy(in state, player);
                byte troops = GameStateHelper.GetPlayerTroopsToPlace(in state, player);

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
                    byte troops = GameStateHelper.GetTerritoryTroops(in state, i);
                    if (troops <= 1) continue;

                    byte[] neighbors = layout.Map.Adjacencies[i];
                    for (int n = 0; n < neighbors.Length; n++)
                    {
                        byte neighborId = neighbors[n];
                        if (GameStateHelper.GetTerritoryOwner(in state, neighborId) != player)
                        {
                            byte diceCount = (byte)Math.Min(3, troops - 1);
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