using System;
using RiskEngine.Decisions;
using RiskEngine.State;
using RiskEngine.State.Generation;

namespace MyEngineCore.Domain.Actions;

/// <summary>
/// Translates a DecisionSelection response back into a single concrete GameAction for pipeline execution.
/// </summary>
public static class ActionFactory
{
    public static GameAction CreateFromSelection(in DecisionSelection selection, ReadOnlySpan<DecisionOption> availableOptions)
    {
        if (selection.OptionIndex >= availableOptions.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(selection), "Selected option index out of bounds.");
        }

        ref readonly var option = ref availableOptions[selection.OptionIndex];

        // Validate parameter boundaries
        if (!option.Parameter.IsEmpty && !option.Parameter.Contains(selection.ChosenParameter))
        {
            throw new InvalidOperationException(
                $"Parameter value {selection.ChosenParameter} is out of valid bounds [{option.Parameter.Min}..{option.Parameter.Max}].");
        }

        // Materialize exactly one concrete GameAction
        return option.Kind switch
        {
            DecisionKind.CardTurnIn => ActionFactoryHelper.TradeCards(option.GetCardTriple().Card1, option.GetCardTriple().Card2, option.GetCardTriple().Card3),
            DecisionKind.Reinforce  => ActionFactoryHelper.Reinforce(option.GetReinforceData().TargetTerritory, selection.ChosenParameter),
            DecisionKind.Attack     => ActionFactoryHelper.Attack(option.GetAttackData().SourceTerritory, option.GetAttackData().TargetTerritory, selection.ChosenParameter),
            DecisionKind.Defend     => ActionFactoryHelper.Defend(selection.ChosenParameter),
            DecisionKind.Conquer    => ActionFactoryHelper.Conquer(selection.ChosenParameter),
            DecisionKind.Fortify    => ActionFactoryHelper.Fortify(option.GetFortifyData().SourceTerritory, option.GetFortifyData().TargetTerritory, selection.ChosenParameter),
            DecisionKind.SkipPhase => ActionFactoryHelper.Skip(),
            DecisionKind.EndTurn => ActionFactoryHelper.EndTurn(),
            _ => throw new NotImplementedException($"Unsupported DecisionKind: {option.Kind}")
        };
    }
}