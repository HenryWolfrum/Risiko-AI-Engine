using System;
using RiskEngine.Decisions;
using RiskEngine.State;

namespace RiskEngine.AI.Bots;

/// <summary>
/// Reference AI implementation that picks a uniformly random legal decision option 
/// and a random scalar parameter value within its valid ParameterSpace.
/// Fully zero-allocation thanks to the Unified Decision Protocol.
/// </summary>
public sealed class RandomBot : IRiskPlayer
{
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
    /// from a base seed and the player's index.
    /// </summary>
    public static RandomBot ForPlayer(int baseSeed, byte playerIndex)
    {
        return new RandomBot(new EngineRandom(baseSeed + (playerIndex + 1) * 104729));
    }

    /// <summary>
    /// Evaluates the legal options and selects a random option index
    /// along with a random parameter value within [MinParam .. MaxParam].
    /// </summary>
    public DecisionSelection Select(in GameState state, ReadOnlySpan<DecisionOption> options)
    {
        if (options.IsEmpty)
        {
            return new DecisionSelection(0, 0);
        }

        // 1. Pick a uniform random decision option
        byte optionIndex = (byte)_rng.Next(0, options.Length);
        ref readonly DecisionOption chosenOption = ref options[optionIndex];

        // 2. Pick a random parameter value within [MinParam .. MaxParam] if parameterized
        byte parameterValue = 0;
        if (!chosenOption.Parameter.IsEmpty)
        {
            parameterValue = (byte)_rng.Next(chosenOption.Parameter.Min, chosenOption.Parameter.Max + 1);
        }

        return new DecisionSelection(optionIndex, parameterValue);
    }
}