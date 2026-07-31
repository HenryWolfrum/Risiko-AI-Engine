using MyEngineCore.Domain.Actions;
using RiskEngine.State.Generation;

using System;
using RiskEngine.Decisions;
using RiskEngine.Domain.Generators;

namespace RiskEngine.State;

public interface IRiskPlayer
{
    /// <summary>
    /// Generates legal decisions zero-alloc on the stack, asks the player to select one,
    /// and materializes it into a state-executable GameAction.
    /// </summary>
    public GameAction DecideAction(in GameState state, GameLayout layout)
    {
        // Allocation-free workspace on the stack for legal decision space
        Span<DecisionOption> buffer = stackalloc DecisionOption[EngineConstants.MAX_DECISION_BUFFER_SIZE];
        
        int count = DecisionGenerator.GenerateLegalDecisions(in state, layout, buffer);
        ReadOnlySpan<DecisionOption> options = buffer.Slice(0, count);

        // Ask the implementation (Bot/MCTS/Human) to select an option and parameter
        DecisionSelection selection = Select(in state, options);

        // Materialize into a concrete GameAction for state execution
        return ActionFactory.CreateFromSelection(in selection, options);
    }

    /// <summary>
    /// Pure decision hook: Evaluates legal options and selects an option index + chosen parameter value.
    /// </summary>
    DecisionSelection Select(in GameState state, ReadOnlySpan<DecisionOption> options);
}