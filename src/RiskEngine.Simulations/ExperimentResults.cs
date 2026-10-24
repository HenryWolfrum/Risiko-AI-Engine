using RiskEngine.Actions;
using RiskEngine.State;

namespace RiskEngine.Simulations;

public sealed class ExperimentResults
{
    public IReadOnlyList<GameState> StartStates { get; }
    public IReadOnlyList<GameState> EndStates { get; }

    public IReadOnlyList<(GameState State, GameAction Action)> StateByAction { get; }

    public ExperimentResults(IReadOnlyList<GameState> startStates, IReadOnlyList<GameState> endStates, IReadOnlyList<(GameState State, GameAction Action)> stateByAction)
    {
        StartStates = startStates;
        EndStates = endStates;
        StateByAction = stateByAction;
    }
}