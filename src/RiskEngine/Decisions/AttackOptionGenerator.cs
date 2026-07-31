using System;
using System.Numerics;
using RiskEngine.Decisions;

namespace RiskEngine.State.Generation;

public static class AttackOptionGenerator
{
    /// <summary>
    /// Generates all valid attack decision options (Source -> Target) for the active player,
    /// parameterized with valid attacker dice ranges, plus a phase skip option.
    /// Uses zero-allocation Bitboard iteration.
    /// </summary>
    public static int Generate(in GameState state, GameLayout layout, Span<DecisionOption> options)
    {
        byte player = state.PlayerTurn;
        int optionCount = 0;

        // Bitmask of all territories owned by the active player
        ulong ownedTerritories = GameStateHelper.GetPlayerTerritoriesBitboard(in state, player);
        ulong sources = ownedTerritories;

        // Iterate over owned source territories via Bitboard
        while (sources != 0)
        {
            byte source = (byte)BitOperations.TrailingZeroCount(sources);
            byte troops = GameStateHelper.GetTerritoryTroops(in state, source);

            // Attacker needs at least 2 troops to attack (1 troop must stay behind)
            if (troops > 1)
            {
                // Risk rule: Max 3 dice, but cannot exceed (troops - 1)
                byte maxDice = (byte)Math.Min(3, troops - 1);
                byte[] neighbors = layout.Map.Adjacencies[source];

                for (int i = 0; i < neighbors.Length; i++)
                {
                    byte target = neighbors[i];

                    // Target MUST belong to an opponent (NOT in ownedTerritories)
                    if ((ownedTerritories & (1UL << target)) == 0)
                    {
                        options[optionCount++] = DecisionOption.Attack(
                            source: source,
                            target: target,
                            minDice: 1,
                            maxDice: maxDice
                        );
                    }
                }
            }

            sources &= sources - 1; // Clear lowest set bit
        }

        // Ending the attack phase voluntarily is always a valid option
        options[optionCount++] = DecisionOption.SkipPhase();
        
        //Ending turn in attack phase voluntarily is alway a valid option
        options[optionCount++] = DecisionOption.EndTurn();


        return optionCount;
    }
}