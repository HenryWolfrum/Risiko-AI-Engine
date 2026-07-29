using System;
using RiskEngine.State;
using RiskEngine.State.Generation;

namespace RiskEngine.AI.Bots;

/// <summary>
/// Reference AI implementation that picks a uniformly random legal action
/// per phase. Used as a baseline opponent and for engine stress-testing.
/// </summary>
public sealed class RandomBot : IRiskPlayer
{
    private const int MaxActionBufferSize = 1024;
    
    private EngineRandom _rng;

    public RandomBot(EngineRandom rng)
    {
        _rng = rng;
    }

    /// <summary>
    /// Creates a RandomBot with its own independent random stream, derived
    /// from a base seed and the player's index. Prefer this over sharing a
    /// single EngineRandom instance across bots, which starts every bot with
    /// an identical internal state and can correlate their decisions on
    /// symmetric maps.
    /// </summary>
    public static RandomBot ForPlayer(int baseSeed, byte playerIndex)
    {
        return new RandomBot(new EngineRandom(baseSeed + (playerIndex + 1) * 104729));
    }

    public GameAction DecideAction(in GameState state, GameLayout layout)
    {
        Span<GameAction> actionBuffer = stackalloc GameAction[MaxActionBufferSize];
        int actionCount;

        switch (state.CurrentPhase)
        {
            case GamePhase.Reinforce:
                actionCount = ReinforcementActionGenerator.Generate(in state, actionBuffer);
                break;

            case GamePhase.Attack:
                actionCount = AttackActionGenerator.Generate(in state, layout, actionBuffer);
                break;

            case GamePhase.Fortify:
                actionCount = FortifyActionGenerator.Generate(in state, layout, actionBuffer);
                break;

            case GamePhase.Conquer:
                return DecideConquerAction(in state);

            case GamePhase.CardTurnIn:
                actionCount = CardTurnInActionGenerator.Generate(in state, layout, actionBuffer);
                break;

            default:
                return new GameAction { Type = ActionType.SkipPhase };
        }

        if (actionCount == 0)
        {
            return new GameAction { Type = ActionType.SkipPhase };
        }

        int selectedIndex = _rng.Next(0, actionCount);
        return actionBuffer[selectedIndex];
    }

    private GameAction DecideConquerAction(in GameState state)
    {
        // 1-3 troops; ConquerExecutor validates and falls back to a safe
        // value (1 troop) if this violates the rules for the current attack.
        byte chosenTroops = (byte)_rng.Next(1, 4);

        return new GameAction
        {
            Type = ActionType.Conquer,
            ConquerTroopCount = chosenTroops
        };
    }
}