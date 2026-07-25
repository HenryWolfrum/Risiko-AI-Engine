using System;
using System.Diagnostics;
using RiskEngine;

Console.WriteLine("=== RISIKO ENGINE BENCHMARK ===");

// Map einmal erstellen
GameLayout game = RiskMapFactory.CreateStandardRiskMap();

// ---------------------------
// Warmup (JIT)
// ---------------------------
GameInitializer.CreateInitialState(game, 42);

// ---------------------------
// Benchmark
// ---------------------------
const int Iterations = 1_000_000;

long allocatedBefore = GC.GetAllocatedBytesForCurrentThread();

var stopwatch = Stopwatch.StartNew();

GameState lastState = default;

for (int i = 0; i < Iterations; i++)
{
    lastState = GameInitializer.CreateInitialState(game, i);
}

stopwatch.Stop();

long allocatedAfter = GC.GetAllocatedBytesForCurrentThread();

long allocated = allocatedAfter - allocatedBefore;

Console.WriteLine();
Console.WriteLine("=== Ergebnisse ===");
Console.WriteLine($"Initialisierungen : {Iterations:N0}");
Console.WriteLine($"Zeit              : {stopwatch.ElapsedMilliseconds} ms");
Console.WriteLine($"Allocation        : {allocated:N0} Bytes");
Console.WriteLine($"Pro Initialisierung: {(double)stopwatch.Elapsed.TotalNanoseconds / Iterations:F1} ns");

// Damit der Compiler die Schleife nicht theoretisch wegoptimieren kann
Console.WriteLine($"Letzter Startspieler: {lastState.PlayerTurn}");

if (allocated == 0)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\n✓ ZERO ALLOCATION");
}
else
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\n⚠ {allocated:N0} Bytes wurden allokiert.");
}

Console.ResetColor();