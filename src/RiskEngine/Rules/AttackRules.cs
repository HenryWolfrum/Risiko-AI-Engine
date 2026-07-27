using System;
using System.Runtime.CompilerServices;

namespace RiskEngine.Validation;

public static class AttackRules
{
    public static ValidationResult Validate(in GameState state, in GameAction action, MapLayout map)
    {
        var source = action.SourceTerritory;
        var target = action.TargetTerritory;
        var player = state.PlayerTurn;

        // Territory range check
        if (source >= map.TerritoryNames.Length || target >= map.TerritoryNames.Length)
            return ValidationResult.Invalid(GameError.InvalidTerritory);

        // Source must belong to attacker
        if (GameStateHelper.GetTerritoryOwner(in state, source) != player)
            return ValidationResult.Invalid(GameError.TerritoryNotOwned);

        // Target must belong to enemy
        if (GameStateHelper.GetTerritoryOwner(in state, target) == player)
            return ValidationResult.Invalid(GameError.InvalidTarget);

        // Territories must be connected
        if (!map.AreNeighbors(source, target))
            return ValidationResult.Invalid(GameError.TerritoriesNotAdjacent);

        var attackerTroops = GameStateHelper.GetTerritoryTroops(in state, source);

        // Need at least 2 troops to attack (1 must stay behind)
        if (attackerTroops < 2)
            return ValidationResult.Invalid(GameError.NotEnoughTroops);

        // Attacker dice validation (Max 3, but at most attackerTroops - 1)
        var maxAttackerDice = (byte)Math.Min(3, attackerTroops - 1);
        if (action.ChosenAttackerDiceCount == 0 || action.ChosenAttackerDiceCount > maxAttackerDice)
            return ValidationResult.Invalid(GameError.InvalidDiceCount);

        return ValidationResult.Valid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidDefenderDice(in GameState state, byte targetTerritory, byte defenderDice)
    {
        if (defenderDice == 0) return false;
        return defenderDice <= GetMaxDefenderDice(in state, targetTerritory);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetMaxDefenderDice(in GameState state, byte targetTerritory)
    {
        var defenderTroops = GameStateHelper.GetTerritoryTroops(in state, targetTerritory);
        return defenderTroops >= 2 ? (byte)2 : (byte)1;
    }
}