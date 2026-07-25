namespace RiskEngine.Mutation;

public static unsafe class FortifyMutator
{
    // Moves troops between owned territories
    public static void Apply(
        ref GameState state,
        in GameAction action)
    {
        byte source = action.SourceTerritory;
        byte target = action.TargetTerritory;
        byte amount = action.TroopCount;

        // Remove troops from source territory
        state.TerritoryTroops[source] -= amount;

        // Add troops to target territory
        state.TerritoryTroops[target] += amount;
    }
}