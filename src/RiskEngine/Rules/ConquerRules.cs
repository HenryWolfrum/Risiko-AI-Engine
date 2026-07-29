namespace RiskEngine.State.Rules;

/// <summary>
/// Contains validation rules for conquering a territory.
/// </summary>
public static class ConquerRules
{
    /// <summary>
    /// Validates troop movement after a successful attack.
    /// </summary>
    public static ValidationResult Validate(in GameState state, in GameAction action,MapLayout map)
    {
        var source = action.SourceTerritory;
        var target = action.TargetTerritory;
        var player = state.PlayerTurn;


        // Territory range check
        if (source >= map.TerritoryCount || target >= map.TerritoryCount)
        {
            return ValidationResult.Invalid(EngineError.InvalidTerritory);
        }


        // Source territory must belong to the attacker
        if (GameStateHelper.GetTerritoryOwner(in state, source) != player)
        {
            return ValidationResult.Invalid(EngineError.TerritoryNotOwned);
        }


        // Target territory must already be conquered.
        // The defender must have no troops left.
        if (GameStateHelper.GetTerritoryTroops(in state, target) != 0)
        {
            return ValidationResult.Invalid(EngineError.InvalidTarget);
        }


        // Source territory must keep at least one troop.
        var sourceTroops = GameStateHelper.GetTerritoryTroops(in state, source);


        if (sourceTroops <= 1)
        {
            return ValidationResult.Invalid(EngineError.NotEnoughTroops);
        }


        // At least one troop must move into the conquered territory.
        if (action.ConquerTroopCount == 0)
        {
            return ValidationResult.Invalid(EngineError.InvalidTroopCount);
        }


        // Cannot move more troops than available.
        // One troop must remain in the source territory.
        var maxMoveable = sourceTroops - 1;


        if (action.ConquerTroopCount > maxMoveable)
        {
            return ValidationResult.Invalid(EngineError.InvalidTroopCount);
        }


        return ValidationResult.Valid();
    }
}