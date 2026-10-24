using RiskEngine.Core;
using RiskEngine.Random;
using RiskEngine.State;

namespace RiskEngine.Simulations;

public class ExperimentRunner
{
    private ExperimentHeader header;
    private EngineRandom random;
    private ExperimentObserver observer;
    private MetricAnalyzer analyzer;

    public ExperimentRunner(ExperimentHeader header,ExperimentObserver observer,MetricAnalyzer analyzer)
    {
        this.header = header;
        random = new EngineRandom(header.seed);
        this.observer = observer;
        this.analyzer = analyzer;
    }

    public void RunNext()
    {
        ulong gameSeed = (ulong) random.Next(0, int.MaxValue);
        
        GameState finalState = GameRunner.PlayGame(header.layout,header.players, gameSeed,observer);
    }

    public void RunExperiment()
    {
        for (int i = 0; i < header.runs; i++)
        {
            RunNext();
        }
    }
    
    
}