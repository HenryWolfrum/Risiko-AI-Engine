namespace RiskEngine.State.Validation;

public static class ReinforceRules
{
    public static ValidationResult Validate(in GameState state, in GameAction action, MapLayout map)
    {
        var player = state.PlayerTurn;


        //Territory out of Range
        if (action.TargetTerritory >= map.TerritoryCount)
            return ValidationResult.Invalid(EngineError.InvalidTerritory);

        //Owner not equal to action Executor
        if (GameStateHelper.GetTerritoryOwner(state, action.TargetTerritory) != player)
            return ValidationResult.Invalid(EngineError.TerritoryNotOwned);

        //No Troops to distribute
        if (action.TroopCount == 0) return ValidationResult.Invalid(EngineError.InvalidTroopCount);

        //More Troops to place than accessible
        if (action.TroopCount > GameStateHelper.GetPlayerTroopsToPlace(state, player))
            return ValidationResult.Invalid(EngineError.NotEnoughTroops);


        return ValidationResult.Valid();
    }
}