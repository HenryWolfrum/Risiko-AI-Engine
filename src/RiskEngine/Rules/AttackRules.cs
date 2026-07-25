namespace RiskEngine.Validation;

public static class AttackRules
{
    public static ValidationResult Validate(in GameState state, in GameAction action, MapLayout map)
    {
        byte source = action.SourceTerritory;
        byte target = action.TargetTerritory;
        byte player = state.PlayerTurn;


        // Territory range check
        if (source >= EngineConstants.DEFAULT_TERRITORY_COUNT || target >= EngineConstants.DEFAULT_TERRITORY_COUNT)
        {
            return ValidationResult.Invalid(GameError.InvalidTerritory);
        }


        // Source must belong to player
        if (state.GetTerritoryOwner(source) != player)
        {
            return ValidationResult.Invalid(GameError.TerritoryNotOwned);
        }


        // Target must belong to enemy
        if (state.GetTerritoryOwner(target) == player)
        {
            return ValidationResult.Invalid(GameError.InvalidTarget);
        }


        // Territories must be connected
        if (!map.AreNeighbors(source, target))
        {
            return ValidationResult.Invalid(GameError.TerritoriesNotAdjacent);
        }


        byte attackerTroops = state.GetTerritoryTroops(source);

        byte defenderTroops = state.GetTerritoryTroops(target);


        // Need at least 2 troops to attack
        if (attackerTroops < 2)
        {
            return ValidationResult.Invalid(GameError.NotEnoughTroops);
        }


        // Validate defender choice first
        if (!IsValidDefenderDice(defenderTroops, action.ChosenDefenderDiceCount))
        {
            return ValidationResult.Invalid(GameError.InvalidDiceCount);
        }


        // Validate attacker response
        if (!IsValidAttackerDice(attackerTroops, action.ChosenDefenderDiceCount, action.ChosenAttackerDiceCount))
        {
            return ValidationResult.Invalid(GameError.InvalidDiceCount);
        }
        
      


        return ValidationResult.Valid();
    }



    private static bool IsValidDefenderDice(byte troops, byte dice)
    {
        if (dice == 0)
            return false;


        byte maxDice = troops >= 2 ? (byte)2 : (byte)1;


        return dice <= maxDice;
    }



    private static bool IsValidAttackerDice(byte troops, byte defenderDice, byte attackerDice)
    {
        if (troops < 2)
            return false;

        if (attackerDice == 0)
            return false;


        byte maxDice;


        // Two troops depend on defender choice
        if (troops == 2)
        {
            maxDice = defenderDice == 1 ? (byte)2 : (byte)1;
        }
        // Three or more troops allow maximum attack
        else
        {
            maxDice = 3;
        }


        return attackerDice <= maxDice;
    }
}