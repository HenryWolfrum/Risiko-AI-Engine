namespace RiskEngine;

public static unsafe class AttackMutator
{
    /// <summary>
    ///     Applies combat casualties (losses) to both source and target territories.
    ///     Does NOT perform conquest logic.
    /// </summary>
    public static void Apply(ref GameState state, in GameAction action, in CombatResult result)
    {
        var attackerTerritory = action.SourceTerritory;
        var defenderTerritory = action.TargetTerritory;

        // Apply attacker losses
        state.TerritoryTroops[attackerTerritory] -= result.AttackerLosses;

        // Apply defender losses with underflow guard
        var defenderTroops = state.TerritoryTroops[defenderTerritory];
        if (result.DefenderLosses >= defenderTroops)
            state.TerritoryTroops[defenderTerritory] = 0;
        else
            state.TerritoryTroops[defenderTerritory] = (byte)(defenderTroops - result.DefenderLosses);
    }
}