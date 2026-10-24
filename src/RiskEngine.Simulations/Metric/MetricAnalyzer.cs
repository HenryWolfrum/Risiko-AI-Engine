namespace RiskEngine.Simulations;

public class MetricAnalyzer
{
    private ExperimentResults results;
    
    private Dictionary<IMetric,double> metricResults;

    private IMetric[] profile;

    public MetricAnalyzer(ExperimentResults results,IMetric[] profile)
    {
        this.results = results;
        this.profile = profile;
    }

    public void CalculateAllMetrics()
    {
        foreach (var metric in profile)
        {
           metricResults[metric] = CalculateMetric(metric);
        }
    }

    private double CalculateMetric(IMetric metric)
    {
        return metric.Calculate(results);
    }
}