namespace RiskEngine.Mutation;

public static unsafe class ReinforceMutator
{
    // Places troops on a territory
    public static void Apply(ref GameState state, in GameAction action)
    {
        byte territory = action.TargetTerritory;
        byte amount = action.TroopCount;
        byte player = state.PlayerTurn;

        // Add troops to territory
        state.TerritoryTroops[territory] += amount;

        // Remove available troops from player pool
        state.PlayerTroopsToPlace[player] -= amount;
    }
}