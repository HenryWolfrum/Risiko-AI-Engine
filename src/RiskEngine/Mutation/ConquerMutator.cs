namespace RiskEngine.Mutation;

public static unsafe class ConquerMutator
{
    /// <summary>
    /// Transfers territory ownership and moves troops into the conquered territory.
    /// </summary>
    public static void Apply(ref GameState state, in GameAction action)
    {
        byte attacker = state.PlayerTurn;

        // Transfer ownership
        GameStateHelper.SetTerritoryOwner(ref state, action.TargetTerritory, attacker);

        // Move troops
        byte sourceTroops = GameStateHelper.GetTerritoryTroops(in state, action.SourceTerritory);
        GameStateHelper.SetTerritoryTroops(ref state, action.SourceTerritory, (byte)(sourceTroops - action.ConquerTroopCount));
        GameStateHelper.SetTerritoryTroops(ref state, action.TargetTerritory, action.ConquerTroopCount);
    }
}