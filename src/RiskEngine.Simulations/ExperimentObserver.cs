using RiskEngine.Actions;
using RiskEngine.Observer;
using RiskEngine.State;

namespace RiskEngine.Simulations;

public sealed class ExperimentObserver:IGameObserver
{
    private bool readGameStart;
    private bool readGameEnd;
    private bool readOnAction;

    public readonly List<GameState> startStates;
    public readonly List<GameState> endStates;
    public readonly List<(GameState,GameAction)> stateByAction;

    public ExperimentObserver(bool readGameStart, bool readGameEnd, bool readOnAction)
    {
        this.readGameStart = readGameStart;
        this.readGameEnd = readGameEnd;
        this.readOnAction = readOnAction;
    }
    
    public void OnGameStarted(in GameState state)
    {
        if (readGameStart)
        {
            startStates.Add(state);
        }
    }

    public void OnActionApplied(in GameState state, in GameAction action)
    {
        if (readOnAction)
        {
            stateByAction.Add((state, action));
        }
    }

    public void OnGameFinished(in GameState state)
    {
        if (readGameEnd)
        {
            endStates.Add(state);
        }
    }

    public List<GameState> GetStartStates()
    {
        return startStates;
    }

    public List<GameState> GetEndStates()
    {
        return endStates;
    }
    
}
