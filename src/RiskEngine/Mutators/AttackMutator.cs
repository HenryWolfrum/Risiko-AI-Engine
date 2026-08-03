using System;
using RiskEngine.Exceptions;

namespace RiskEngine.State;

/// <summary>
/// Applies combat casualties (losses) to both source and target territories.
/// Enforces strict invariants and fails fast on impossible casualty numbers.
/// Does NOT perform conquest logic.
/// </summary>
public static class AttackMutator
{
    /// <summary>
    /// Applies combat casualties to attacker and defender territories.
    /// </summary>
    public static void Apply(ref GameState state, in GameAction action, in CombatResult result)
    {
        var attackerTerritory = action.SourceTerritory;
        var defenderTerritory = action.TargetTerritory;

        // Fetch current troop counts.
        var attackerTroops = GameStateHelper.GetTerritoryTroops(in state, attackerTerritory);
        var defenderTroops = GameStateHelper.GetTerritoryTroops(in state, defenderTerritory);

        // Fail-fast Guard 1: Attacker territory can NEVER drop to 0 troops during combat.
        if (result.AttackerLosses >= attackerTroops)
        {
            throw new InvalidGameActionException(
                $"Combat casualties caused attacker territory #{attackerTerritory} to drop to 0 or negative troops!\n" +
                $"  • Source Territory: #{attackerTerritory} (Available troops: {attackerTroops})\n" +
                $"  • Attacker Losses:  {result.AttackerLosses}\n" +
                $"  • Invariant Check:  Attacker must retain at least 1 troop on source territory."
            );
        }

        // Fail-fast Guard 2: Defender losses cannot exceed defender troops present.
        if (result.DefenderLosses > defenderTroops)
        {
            throw new InvalidGameActionException(
                $"Combat casualties exceeded defender troops on target territory #{defenderTerritory}!\n" +
                $"  • Target Territory: #{defenderTerritory} (Current troops: {defenderTroops})\n" +
                $"  • Defender Losses:  {result.DefenderLosses}"
            );
        }

        // Apply validated losses cleanly without silent clamping.
        var attackerNewAmount = (byte)(attackerTroops - result.AttackerLosses);
        var defenderNewAmount = (byte)(defenderTroops - result.DefenderLosses);

        GameStateHelper.SetTerritoryTroops(ref state, attackerTerritory, attackerNewAmount);
        GameStateHelper.SetTerritoryTroops(ref state, defenderTerritory, defenderNewAmount);
    }
}