using System.Numerics;
using RiskEngine.Decisions;
using RiskEngine.State;


public static class ReinforcementOptionGenerator
{
    /// <summary>
    /// Generates all valid reinforcement decision options for the active player using bitboard iteration.
    /// Each target territory option is parameterized with the valid range of troops [1..availableTroops] 
    /// that can be placed in a single decision step.
    /// </summary>
    public static int Generate(in GameState state, Span<DecisionOption> options)
    {
        byte player = state.PlayerTurn;
        int optionCount = 0;

        // Fetch remaining unplaced troops for the active player
        byte availableTroops = GameStateHelper.GetPlayerTroopsToPlace(in state, player);

        // Safety check: If no troops are left to place, return 0 options
        if (availableTroops == 0)
        {
            return 0;
        }

        // Fetch the bitmask of all territories owned by the active player
        ulong ownedTerritories = GameStateHelper.GetPlayerTerritoriesBitboard(in state, player);

        // Iterate strictly over set bits (owned territories)
        while (ownedTerritories != 0)
        {
            // Extract the index of the lowest set bit (0-63)
            byte territory = (byte)BitOperations.TrailingZeroCount(ownedTerritories);

            options[optionCount++] = DecisionOption.Reinforce(
                target: territory,
                minTroops: 1,
                maxTroops: availableTroops
            );

            // Clear the lowest set bit to move to the next territory
            ownedTerritories &= ownedTerritories - 1;
        }

        return optionCount;
    }
}