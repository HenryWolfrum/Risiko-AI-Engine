using System;
 using System.Collections.Generic;
 using System.Runtime.InteropServices;
 using RiskEngine.Decisions;
 using RiskEngine.State;
 using RiskEngine.State.Generation;
 
 namespace RiskEngine.Domain.Generators;
 
 /// <summary>
 /// High-performance dispatcher that generates the complete Decision Space for any game state
 /// without executing memory allocations.
 /// </summary>
 public static class DecisionGenerator
 {
     /// <summary>
     /// Fills the output span with all legal decision options for the current player turn and phase.
     /// Returns the total number of options generated.
     /// </summary>
     public static int GenerateLegalDecisions(in GameState state, GameLayout layout, Span<DecisionOption> outputBuffer)
     {
         return state.CurrentPhase switch
         {
             GamePhase.CardTurnIn  => CardTurnInOptionGenerator.Generate(in state, layout, outputBuffer),
             GamePhase.Reinforce => ReinforcementOptionGenerator.Generate(in state, outputBuffer),
             GamePhase.Attack    => AttackOptionGenerator.Generate(in state, layout, outputBuffer),
             GamePhase.Defend    => DefendOptionGenerator.Generate(in state, outputBuffer),
             GamePhase.Conquer   => ConquerOptionGenerator.Generate(in state, outputBuffer),
             GamePhase.Fortify   => FortifyOptionGenerator.Generate(in state, layout, outputBuffer),
             _ => 0
         };
     }
 
     /// <summary>
     /// Optional overload for legacy code working with List&lt;DecisionOption&gt;.
     /// Uses a stack-allocated buffer internally to remain Zero-Allocation until adding to the list.
     /// </summary>
     public static void GenerateLegalDecisions(in GameState state, GameLayout layout, List<DecisionOption> outputList)
     {
         outputList.Clear();
 
         // 254 options cover the theoretical max for any single phase in Risk
         Span<DecisionOption> buffer = stackalloc DecisionOption[EngineConstants.MAX_DECISION_BUFFER_SIZE];
         
         int count = GenerateLegalDecisions(in state, layout, buffer);
 
         for (int i = 0; i < count; i++)
         {
             outputList.Add(buffer[i]);
         }
     }
 }