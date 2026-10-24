using RiskEngine.Layout;
using RiskEngine.Player;

namespace RiskEngine.Simulations;

/// <summary>
/// Represents the necessary meta information of an Experiment
/// </summary>
public struct ExperimentHeader
{
    public readonly ulong seed;

    public readonly int runs;

    public readonly IRiskPlayer[] players;

    public readonly GameLayout layout;
}