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
        var currentTroops = GameStateHelper.GetTerritoryTroops(in state, territory);
        GameStateHelper.SetTerritoryTroops(ref state, territory, (byte)(currentTroops + amount));
        
        //Remove from Troop Bank
        var availableTroops = GameStateHelper.GetPlayerTroopsToPlace(in state, player);
        GameStateHelper.SetPlayerTroopsToPlace(ref state, player, (byte)(availableTroops - amount));
    }
}