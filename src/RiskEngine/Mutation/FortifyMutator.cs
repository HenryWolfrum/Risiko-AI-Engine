namespace RiskEngine.Mutation;

public static class FortifyMutator
{
    public static void Apply(ref GameState state, in GameAction action)
    {
        byte source = action.SourceTerritory;
        byte target = action.TargetTerritory;
        byte troops = action.TroopCount;
        
        //Source removes a count of troops
        GameStateHelper.SetTerritoryTroops(ref state,source, (byte)(GameStateHelper.GetTerritoryTroops(state,source) - troops));

        //Target adds a count of troops
        GameStateHelper.SetTerritoryTroops(ref state,target, (byte)(GameStateHelper.GetTerritoryTroops(state,target) + troops));
    }
}