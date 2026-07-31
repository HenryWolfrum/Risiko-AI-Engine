using System;
using RiskEngine.Decisions;

namespace RiskEngine.State.Generation;

public static class ConquerOptionGenerator
{
    /// <summary>
    /// Generates the single parameterized conquer (occupation) move option.
    /// Specifies the min and max troops that can/must be moved into the newly conquered territory.
    /// </summary>
    public static int Generate(in GameState state, Span<DecisionOption> options)
    {
        // Truppen des Angreifers auf dem Ursprungsterritorium (mind. 1 Truppe muss zurückbleiben)
        byte sourceTroops = GameStateHelper.GetTerritoryTroops(in state, state.AttackerTerritory);
        byte maxTroops = (byte)(sourceTroops - 1);


        byte minTroops = 1;

        options[0] = DecisionOption.Conquer(
            minTroops: minTroops,
            maxTroops: maxTroops
        );

        return 1; // Liefert genau 1 parametrisierte Option zurück
    }
}