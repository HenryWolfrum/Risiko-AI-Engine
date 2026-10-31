using RiskEngine.AI.Configuration;
using RiskEngine.Layout;
using RiskEngine.Player;

namespace RiskEngine.Simulations;

/// <summary>
/// Represents the necessary meta information of an Experiment
/// </summary>
public struct ExperimentDefinition
{
    public readonly ulong MasterSeed;

    public readonly int GameCount;

    public readonly IReadOnlyList<Participant> Participants;

    public readonly GameLayout Layout;

    public readonly SeatRotation Rotation;
}