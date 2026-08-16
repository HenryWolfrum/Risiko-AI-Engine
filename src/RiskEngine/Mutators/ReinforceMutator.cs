namespace RiskEngine.State.Mutation;

public static class ReinforceMutator
{
    // Places troops on a territory
    public static void Apply(ref GameState state, in GameAction action)
    {
        //Get values
        var territory = action.TargetTerritory;
        var amount = action.TroopCount;
        var player = state.PlayerTurn;
        
        //Add to Territory
        GameStateHelper.AddTerritoryTroops(ref state,territory,amount);
        
        //Remove from Troop Bank
        GameStateHelper.SubPlayerTroopsToPlace(ref state, player, amount);
    }
}