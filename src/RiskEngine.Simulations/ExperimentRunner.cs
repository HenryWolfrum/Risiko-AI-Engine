using RiskEngine.Simulations;
using RiskEngine.State;

public static class ExperimentRunner
{
    public static Metric Run(Metric template, int games, int threadCount,
        Func<int, GameState> playGame)
    {
        // 1. Jeder Thread bekommt seine eigene, leere Kopie der Metrik.
        var perThread = new Metric[threadCount];
        for (int t = 0; t < threadCount; t++)
            perThread[t] = template.CreateEmpty();

        // 2. Threads starten. Thread t spielt die Spiele t, t+threadCount, t+2*threadCount, ...
        var threads = new Thread[threadCount];
        for (int t = 0; t < threadCount; t++)
        {
            int threadIndex = t;   // eigene Kopie, damit der Thread nicht die Schleifenvariable teilt
            threads[t] = new Thread(() =>
            {
                var myMetric = perThread[threadIndex];
                for (int g = threadIndex; g < games; g += threadCount)
                {
                    GameState finalState = playGame(g);
                    myMetric.Add(finalState);
                }
            });
            threads[t].Start();
        }

        // 3. Warten, bis alle Threads fertig sind.
        foreach (var thread in threads)
            thread.Join();

        // 4. Alle Teilergebnisse in die erste Metrik zusammenführen.
        var total = perThread[0];
        for (int t = 1; t < threadCount; t++)
            total.Merge(perThread[t]);

        return total;
    }
}