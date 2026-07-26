namespace RiskEngine.Mutation;

public static class FortifyMutator
{
    public static void Apply(ref GameState state, in GameAction action)
    {
        byte source = action.SourceTerritory;
        byte target = action.TargetTerritory;
        byte troops = action.TroopCount;
        
        //Source removes a count of troops
        state.SetTerritoryTroops(source, (byte)(state.GetTerritoryTroops(source) - troops));

        //Target adds a count of troops
        state.SetTerritoryTroops(target, (byte)(state.GetTerritoryTroops(target) + troops));
    }
}