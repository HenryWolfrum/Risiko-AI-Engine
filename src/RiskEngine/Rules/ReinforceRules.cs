namespace RiskEngine.Validation;

public static class ReinforceRules
{
    public static ValidationResult Validate(in GameState state, in GameAction action)
    {
        byte player = state.PlayerTurn;


        //Territory out of Range
        if (action.SourceTerritory >= EngineConstants.DEFAULT_TERRITORY_COUNT)
        {
            return ValidationResult.Invalid(GameError.InvalidTerritory);
        }

        //Owner not equal to action Executor
        if (state.GetTerritoryOwner(action.SourceTerritory) != player)
        {
            return ValidationResult.Invalid(GameError.TerritoryNotOwned);
        }

        //No Troops to distribute
        if (action.TroopCount == 0)
        {
            return ValidationResult.Invalid(GameError.InvalidTroopCount);
        }

        //More Troops to place than accessible
        if (action.TroopCount > state.GetPlayerTroopsToPlace(player))
        {
            return ValidationResult.Invalid(GameError.NotEnoughTroops);
        }


        return ValidationResult.Valid();
    }
}