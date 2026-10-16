using RiskEngine.Actions;
using RiskEngine.State;

namespace RiskEngine.Mutators;

public static class FortifyMutator
{
    public static void Apply(ref GameState state, in GameAction action)
    {
        var source = action.SourceTerritory;
        var target = action.TargetTerritory;
        var troops = action.TroopCount;

        //Source removes a count of troops
        GameStateHelper.SubTerritoryTroops(ref state, source, troops);

        //Target adds a count of troops
        GameStateHelper.AddTerritoryTroops(ref state, target, troops);
    }
}