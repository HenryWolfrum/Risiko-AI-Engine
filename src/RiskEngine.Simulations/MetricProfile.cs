using RiskEngine.State;

namespace RiskEngine.Simulations;

public sealed class MetricProfile
{
    private readonly List<Metric> _metrics;

    public MetricProfile(IEnumerable<Metric> metrics)
    {
        _metrics = metrics.ToList();
    }

 
    public MetricProfile CreateEmpty() =>
        new MetricProfile(_metrics.Select(m => m.CreateEmpty()));


    public void Add(GameState finalState)
    {
        foreach (var metric in _metrics)
            metric.Add(finalState);
    }

  
    public void Merge(MetricProfile other)
    {
        for (int i = 0; i < _metrics.Count; i++)
            _metrics[i].Merge(other._metrics[i]);
    }

    public IReadOnlyList<(string Name, double Value)> Results() =>
        _metrics.Select(m => (m.Name, m.Result())).ToList();
}