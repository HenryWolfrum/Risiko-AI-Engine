namespace RiskEngine.Simulations;

public interface IMetric
{
    public string Name { get; }

    public bool needsStart { get;}
    public bool needsEnd { get; }
    public bool needsTransition { get; }

    public double Calculate(ExperimentResults results);
}