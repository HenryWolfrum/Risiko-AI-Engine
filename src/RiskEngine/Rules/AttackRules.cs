namespace RiskEngine.Validation;

public static class AttackRules
{
    public static ValidationResult Validate(in GameState state, in GameAction action, MapLayout map)
    {
        byte source = action.SourceTerritory;
        byte target = action.TargetTerritory;
        byte player = state.PlayerTurn;

        //Source or Target out of Range
        if (source >= EngineConstants.DEFAULT_TERRITORY_COUNT || target >= EngineConstants.DEFAULT_TERRITORY_COUNT)
        {
            return ValidationResult.Invalid(GameError.InvalidTerritory);
        }

        //Source is not owned by player
        if (state.GetTerritoryOwner(source) != player)
        {
            return ValidationResult.Invalid(
                GameError.TerritoryNotOwned);
        }

        //Target is owned by player
        if (state.GetTerritoryOwner(target) == player)
        {
            return ValidationResult.Invalid(GameError.InvalidTarget);
        }

        //Not Adjacent Territories
        if (!map.AreNeighbors(source, target))
        {
            return ValidationResult.Invalid(GameError.TerritoriesNotAdjacent);
        }

        //Only one Attacking Troop
        if (state.GetTerritoryTroops(source) < 2)
        {
            return ValidationResult.Invalid(GameError.NotEnoughTroops);
        }


        return ValidationResult.Valid();
    }
}