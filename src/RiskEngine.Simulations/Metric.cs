using RiskEngine.State;

namespace RiskEngine.Simulations;

public abstract class Metric
{
    public string Name { get; }
    protected Metric(string name) { Name = name; }

    public abstract Metric CreateEmpty();   
    public abstract void Add(GameState finalState);  
    public abstract void Merge(Metric other);  
    public abstract double Result();     
}