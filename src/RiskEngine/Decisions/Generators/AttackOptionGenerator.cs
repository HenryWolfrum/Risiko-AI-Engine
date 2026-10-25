using System.Numerics;
using RiskEngine.Constants;
using RiskEngine.Layout;
using RiskEngine.State;

namespace RiskEngine.Decisions.Generators;

public static class AttackOptionGenerator
{
    /// <summary>
    ///     Generates all valid attack decision options (Source -> Target) for the active player,
    ///     parameterized with valid attacker dice ranges, plus a phase skip option.
    ///     Uses zero-allocation Bitboard iteration.
    /// </summary>
    public static int Generate(in GameState state, GameLayout layout, Span<DecisionOption> options)
    {
        var player = GameStateHelper.GetPlayerOnTurn(in state);
        var optionCount = 0;

        // Bitmask of all territories owned by the active player
        var ownedTerritories = GameStateHelper.GetPlayerTerritoriesBitboard(in state, player);
        var sources = ownedTerritories;

        // Iterate over owned source territories via Bitboard
        while (sources != 0)
        {
            var source = (byte)BitOperations.TrailingZeroCount(sources);
            var troops = GameStateHelper.GetTerritoryTroops(in state, source);

            // Attacker needs at least 2 troops to attack (1 troop must stay behind)
            if (troops > 1)
            {
                // Risk rule: Max dice, but cannot exceed (troops - 1)
                var maxDice = (byte)Math.Min(EngineConstants.MAX_ATTACKER_DICE, troops - 1);
                var neighbors = layout.Map.Adjacencies[source];

                for (var i = 0; i < neighbors.Length; i++)
                {
                    var target = neighbors[i];

                    // Target MUST belong to opponent AND MUST have at least 1 defender troop
                    if ((ownedTerritories & (1UL << target)) == 0 &&
                        GameStateHelper.GetTerritoryTroops(in state, target) > 0)
                        options[optionCount++] = DecisionOption.Attack(
                            source,
                            target,
                            1,
                            maxDice
                        );
                }
            }

            sources &= sources - 1; // Clear lowest set bit
        }

        // Ending the attack phase voluntarily is always a valid option
        options[optionCount++] = DecisionOption.SkipPhase();

        // Ending turn in attack phase voluntarily is always a valid option
        options[optionCount++] = DecisionOption.EndTurn();

        return optionCount;
    }
}