using RiskEngine.State;

namespace RiskEngine.Simulations;

public class AverageRoundCountMetric:IMetric
{
    public string Name => "avg. Number of Rounds";
    
    public bool needsStart => false;
    public bool needsEnd => true;
    public bool needsTransition => false;

    public double Calculate(ExperimentResults result)
    {
        var finalStateList = result.EndStates;

        int sum = 0;

        foreach (GameState finalState in finalStateList)
        {
            sum+=finalState.CurrentRound;
        }
        
        return (double)sum/finalStateList.Count;
        
    }
}