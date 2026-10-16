using RiskEngine.State;

namespace RiskEngine.Decisions.Generators;

public static class DefendOptionGenerator
{
    /// <summary>
    ///     Generates the single parameterized defend option.
    ///     The defender can roll 1 or 2 dice depending on troops present on the target territory.
    /// </summary>
    public static int Generate(in GameState state, Span<DecisionOption> options)
    {
        // Hole die Truppenstärke des angegriffenen Territoriums
        var defenderTroops = GameStateHelper.GetTerritoryTroops(in state, state.DefenderTerritory);

        // Risikoregel: 2 Würfel ab 2 Truppen, sonst max. 1 Würfel
        var maxDice = defenderTroops >= 2 ? (byte)2 : (byte)1;

        options[0] = DecisionOption.Defend(
            1,
            maxDice
        );

        return 1; // Liefert genau 1 parametrisierte Option zurück
    }
}