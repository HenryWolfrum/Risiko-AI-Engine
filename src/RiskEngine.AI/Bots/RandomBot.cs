using System;
using RiskEngine.State;
using RiskEngine.State.Generation;

namespace RiskEngine.AI.Bots;

/// <summary>
/// Reference AI implementation that picks a uniformly random legal action
/// per phase. Used as a baseline opponent and for engine stress-testing.
///
/// Note: the action generators only produce the *structural* part of an
/// action (which territory/territories are involved). Free parameters that
/// represent policy decisions - troop counts and dice counts - are
/// intentionally left at their default (0) by the generators, so the
/// state space of "all legal actions" doesn't explode. It is the player's
/// (this bot's) responsibility to fill those in before returning the
/// action. Forgetting this makes every such action fail rule validation
/// (TroopCount == 0 / ChosenAttackerDiceCount == 0 are always invalid),
/// which silently no-ops Attack/Fortify and forces Reinforce into its
/// dumb fallback (all troops on the first owned territory) - the engine
/// still "runs", it just never does anything meaningful.
/// </summary>
public sealed class RandomBot : IRiskPlayer
{
    private const int MaxActionBufferSize = 1024;

    // NOT readonly: EngineRandom is a mutable struct. A readonly field would
    // force the compiler to make a defensive copy on every call to Next(),
    // so _state would never actually advance and the bot would always pick
    // the same action for a given actionCount. Do not add `readonly` back.
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
                return actionCount == 0
                    ? new GameAction { Type = ActionType.SkipPhase }
                    : FillReinforce(in state, actionBuffer[_rng.Next(0, actionCount)]);

            case GamePhase.Attack:
                actionCount = AttackActionGenerator.Generate(in state, layout, actionBuffer);
                return actionCount == 0
                    ? new GameAction { Type = ActionType.SkipPhase }
                    : FillAttack(in state, actionBuffer[_rng.Next(0, actionCount)]);

            case GamePhase.Fortify:
                actionCount = FortifyActionGenerator.Generate(in state, layout, actionBuffer);
                return actionCount == 0
                    ? new GameAction { Type = ActionType.SkipPhase }
                    : FillFortify(in state, actionBuffer[_rng.Next(0, actionCount)]);

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

    /// <summary>
    /// Picks a random troop count in [1, troopsToPlace] for the chosen
    /// territory. The executor calls DecideAction repeatedly until all
    /// reinforcement troops are placed, so it's fine (and more "random")
    /// to not dump everything at once.
    /// </summary>
    private GameAction FillReinforce(in GameState state, GameAction action)
    {
        byte troopsToPlace = GameStateHelper.GetPlayerTroopsToPlace(in state, state.PlayerTurn);
        action.TroopCount = (byte)_rng.Next(1, troopsToPlace + 1);
        return action;
    }

    /// <summary>
    /// Picks a random legal attacker dice count: 1..min(3, attackerTroops-1).
    /// SkipPhase entries from the generator pass through untouched.
    /// </summary>
    private GameAction FillAttack(in GameState state, GameAction action)
    {
        if (action.Type != ActionType.Attack)
        {
            return action;
        }

        byte attackerTroops = GameStateHelper.GetTerritoryTroops(in state, action.SourceTerritory);
        byte maxDice = (byte)Math.Min(3, attackerTroops - 1);
        action.ChosenAttackerDiceCount = (byte)_rng.Next(1, maxDice + 1);
        return action;
    }

    /// <summary>
    /// Picks a random troop count to relocate: 1..(sourceTroops-1), since
    /// at least one troop must remain behind. EndTurn entries from the
    /// generator pass through untouched.
    /// </summary>
    private GameAction FillFortify(in GameState state, GameAction action)
    {
        if (action.Type != ActionType.Fortify)
        {
            return action;
        }

        byte sourceTroops = GameStateHelper.GetTerritoryTroops(in state, action.SourceTerritory);
        byte maxMovable = (byte)(sourceTroops - 1);
        action.TroopCount = (byte)_rng.Next(1, maxMovable + 1);
        return action;
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