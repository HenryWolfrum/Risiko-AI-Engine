using System.Numerics;

namespace RiskEngine.State.Generation;

public static class ReinforcementActionGenerator
{
    /// <summary>
    /// Generates all valid reinforcement target territories for the active player using bitboard iteration.
    /// The troop count decision is left to the AI policy.
    /// </summary>
    public static int Generate(in GameState state, Span<GameAction> actions)
    {
        byte player = state.PlayerTurn;
        int actionCount = 0;

        // Fetch the bitmask of all territories owned by the active player
        ulong ownedTerritories = GameStateHelper.GetPlayerTerritoriesBitboard(in state, player);

        // Iterate strictly over set bits (owned territories)
        while (ownedTerritories != 0)
        {
            // Extract the index of the lowest set bit (0-63)
            byte territory = (byte)BitOperations.TrailingZeroCount(ownedTerritories);

            actions[actionCount++] = new GameAction
            {
                Type = ActionType.Reinforce,
                TargetTerritory = territory
            };

            // Clear the lowest set bit to move to the next territory
            ownedTerritories &= ownedTerritories - 1;
        }

        return actionCount;
    }
}