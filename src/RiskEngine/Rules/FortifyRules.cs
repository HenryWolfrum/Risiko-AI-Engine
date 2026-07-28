using RiskEngine;

public static class FortifyRules
{
    public static ValidationResult Validate(in GameState state, in GameAction action, MapLayout map)
    {
        var source = action.SourceTerritory;
        var target = action.TargetTerritory;
        var player = state.PlayerTurn;

        //Source or Target out of Range
        if (source >= map.TerritoryCount || target >= map.TerritoryCount)
            return ValidationResult.Invalid(GameError.InvalidTerritory);

        //Source is equal to Target
        if (source == target) return ValidationResult.Invalid(GameError.InvalidTarget);

        //Source or Target not owned
        if (GameStateHelper.GetTerritoryOwner(state, source) != player ||
            GameStateHelper.GetTerritoryOwner(state, target) != player)
            return ValidationResult.Invalid(GameError.TerritoryNotOwned);

        //Has only zero troops to move
        if (action.TroopCount == 0) return ValidationResult.Invalid(GameError.InvalidAction);

        var troops = GameStateHelper.GetTerritoryTroops(state, source);

        //Move more troops than possible
        if (action.TroopCount >= troops) return ValidationResult.Invalid(GameError.NotEnoughTroops);

        //No existing path
        if (!MapTraverser.HasPath(state, map, source, target, player))
            return ValidationResult.Invalid(GameError.NoPathFound);

        return ValidationResult.Valid();
    }
}