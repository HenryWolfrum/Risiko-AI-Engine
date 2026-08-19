using System;
using RiskEngine.Decisions;
using RiskEngine.State;
using RiskEngine.State.Generation;

namespace RiskEngine.AI.Bots;

/// <summary>
/// Aggressive AI that prioritizes attack, maximum troop commitments, and forward pushes.
/// </summary>
public sealed class AggroBot : IRiskPlayer
{

    public DecisionSelection Select(in GameState state, ReadOnlySpan<DecisionOption> options)
    {
        if (options.IsEmpty)
            throw new InvalidOperationException("AggroBot received an empty decision option list.");

        // 1. Pflichtentscheidungen: Verteidigung & Nachrücken nach Eroberung
        int conquerIndex = FindFirst(options, DecisionKind.Conquer);
        if (conquerIndex != -1)
            return SelectMax(options, (byte)conquerIndex);

        int defendIndex = FindFirst(options, DecisionKind.Defend);
        if (defendIndex != -1)
            return SelectMax(options, (byte)defendIndex);

        int cardIndex = FindFirst(options, DecisionKind.CardTurnIn);
        if (cardIndex != -1)
            return SelectMax(options, (byte)cardIndex);

        // 2. Offensiv-Aktion: Angriff bevorzugen
        int attackIndex = FindFirst(options, DecisionKind.Attack);
        if (attackIndex != -1)
            return SelectMax(options, (byte)attackIndex);

        // 3. Aufbau-Aktion: Verstärkung platzieren
        int reinforceIndex = FindFirst(options, DecisionKind.Reinforce);
        if (reinforceIndex != -1)
            return SelectMax(options, (byte)reinforceIndex);

        // 4. Manöver-Aktion: Truppen verschieben
        int fortifyIndex = FindFirst(options, DecisionKind.Fortify);
        if (fortifyIndex != -1)
            return SelectMax(options, (byte)fortifyIndex);

        // 5. Passiv-Aktionen: Phase überspringen oder Zug beenden
        int skipIndex = FindFirst(options, DecisionKind.SkipPhase);
        if (skipIndex != -1)
            return new DecisionSelection((byte)skipIndex, 0);

        int endTurnIndex = FindFirst(options, DecisionKind.EndTurn);
        if (endTurnIndex != -1)
            return new DecisionSelection((byte)endTurnIndex, 0);

        // Fallback: Die allererste Option wählen
        return SelectMax(options, 0);
    }

    private static int FindFirst(ReadOnlySpan<DecisionOption> options, DecisionKind kind)
    {
        for (int i = 0; i < options.Length; i++)
        {
            if (options[i].Kind == kind)
                return i;
        }
        return -1;
    }

    private static DecisionSelection SelectMax(ReadOnlySpan<DecisionOption> options, byte index)
    {
        ref readonly DecisionOption option = ref options[index];
        byte param = option.Parameter.IsEmpty ? (byte)0 : option.Parameter.Max;
        return new DecisionSelection(index, param);
    }
}