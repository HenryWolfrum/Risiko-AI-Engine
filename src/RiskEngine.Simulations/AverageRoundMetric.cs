using RiskEngine.State;

namespace RiskEngine.Simulations;

public sealed class AverageRoundsMetric : Metric
{
    private long _sumRounds;
    private long _games;

    public AverageRoundsMetric() : base("Durchschnittliche Rundenzahl") { }

    public override Metric CreateEmpty() => new AverageRoundsMetric();

    public override void Add(GameState finalState)
    {
        _sumRounds += finalState.CurrentRound;
        _games++;
    }

    public override void Merge(Metric other)
    {
        var o = (AverageRoundsMetric)other;
        _sumRounds += o._sumRounds;
        _games += o._games;
    }

    public override double Result() => (double)_sumRounds / _games;
}