namespace RiskEngine.State.Mutation;

/// <summary>
/// Applies the state changes caused by conquering a territory.
/// Assumes that the action was already validated.
/// </summary>
public static class ConquerMutator
{
    /// <summary>
    /// Transfers territory ownership and moves attacking troops
    /// into the conquered territory.
    /// </summary>
    public static void Apply(ref GameState state, in GameAction action)
    {
        var attacker = state.PlayerTurn;


        // Get current troop count from the attacking territory.
        var sourceTroops = GameStateHelper.GetTerritoryTroops(in state, action.SourceTerritory);


        // Remove moved troops from the source territory.
        GameStateHelper.SetTerritoryTroops(ref state, action.SourceTerritory, (byte)(sourceTroops - action.TroopCount));


        // Transfer ownership of the conquered territory.
        GameStateHelper.SetTerritoryOwner(ref state, action.TargetTerritory, attacker);


        // Place moved troops into the conquered territory.
        GameStateHelper.SetTerritoryTroops(ref state, action.TargetTerritory, action.TroopCount);
    }
}