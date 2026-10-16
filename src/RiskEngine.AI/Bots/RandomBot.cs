using RiskEngine.Decisions;
using RiskEngine.Player;
using RiskEngine.Random;
using RiskEngine.State;

namespace RiskEngine.AI.Bots;

/// <summary>
///     Reference AI implementation that selects a uniformly random legal decision.
/// </summary>
public sealed class RandomBot : IRiskPlayer
{
    private readonly EngineRandom _rng;

    internal RandomBot(EngineRandom rng)
    {
        _rng = rng;
    }

    public DecisionSelection Select(in GameState state, ReadOnlySpan<DecisionOption> options)
    {
        if (options.IsEmpty)
            throw new InvalidOperationException("RandomBot received an empty decision option list.");

        var optionIndex = (byte)_rng.Next(0, options.Length);

        ref readonly var option = ref options[optionIndex];

        byte parameter = 0;

        if (!option.Parameter.IsEmpty) parameter = (byte)_rng.Next(option.Parameter.Min, option.Parameter.Max + 1);

        return new DecisionSelection(optionIndex, parameter);
    }
}