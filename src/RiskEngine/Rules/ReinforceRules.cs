namespace RiskEngine.Validation;

public static class ReinforceRules
{
    public static ValidationResult Validate(in GameState state, in GameAction action, MapLayout map)
    {
        var player = state.PlayerTurn;


        //Territory out of Range
        if (action.SourceTerritory >= map.TerritoryCount)
            return ValidationResult.Invalid(GameError.InvalidTerritory);

        //Owner not equal to action Executor
        if (GameStateHelper.GetTerritoryOwner(state, action.SourceTerritory) != player)
            return ValidationResult.Invalid(GameError.TerritoryNotOwned);

        //No Troops to distribute
        if (action.TroopCount == 0) return ValidationResult.Invalid(GameError.InvalidTroopCount);

        //More Troops to place than accessible
        if (action.TroopCount > GameStateHelper.GetPlayerTroopsToPlace(state, player))
            return ValidationResult.Invalid(GameError.NotEnoughTroops);


        return ValidationResult.Valid();
    }
}