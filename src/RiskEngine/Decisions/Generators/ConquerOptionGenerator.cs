using System;
using RiskEngine.Decisions;

namespace RiskEngine.State.Generation;

public static class ConquerOptionGenerator
{
    /// <summary>
    /// Generates the single parameterized conquer (occupation) move option.
    /// Specifies the source, target, and troop bounds (min to max) that can/must be moved.
    /// </summary>
    public static int Generate(in GameState state, Span<DecisionOption> options)
    {
        var source = state.AttackerTerritory;
        var target = state.DefenderTerritory;

        // Fail-fast Guard 1: Cache auf Gültigkeit prüfen (#255 fangen, BEVOR der Underflow passiert)
        if (source == EngineConstants.NO_VALUE || target == EngineConstants.NO_VALUE)
        {
            throw new InvalidOperationException(
                $"Cannot generate conquer options! State cache is uninitialized.\n" +
                $"  • AttackerTerritory: #{source}\n" +
                $"  • DefenderTerritory: #{target}"
            );
        }

        // Truppen des Angreifers auf dem Ursprungsterritorium (in state für Zero-Allocation)
        byte sourceTroops = GameStateHelper.GetTerritoryTroops(in state, source);

        // Fail-fast Guard 2: Mindestens 2 Truppen erforderlich (1 muss stehen bleiben)
        if (sourceTroops < 2)
        {
            throw new InvalidOperationException(
                $"Cannot generate conquer options for territory #{source} with only {sourceTroops} troops!\n" +
                $"At least 2 troops are required on source territory to perform a move."
            );
        }

        byte maxTroops = (byte)(sourceTroops - 1);
        byte minTroops = 1; // Regelkonform mindestens 1 Truppe

        options[0] = DecisionOption.Conquer(
            source: source,
            target: target,
            minTroops: minTroops,
            maxTroops: maxTroops
        );

        return 1;
    }
}