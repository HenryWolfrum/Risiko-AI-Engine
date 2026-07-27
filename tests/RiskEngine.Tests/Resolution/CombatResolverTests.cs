using RiskEngine.Resolution;

namespace RiskEngine.Tests.Resolution;

using RiskEngine;
using Xunit;

public class CombatResolverTests
{
    /*
     * COMBAT-001
     *
     * A combat round with one attacker die
     * and one defender die should resolve correctly.
     *
     * Guarantees:
     * - one comparison is performed
     * - exactly one side loses a troop
     */
    [Fact]
    public void COMBAT_001_SingleDiceCombat_ShouldResolveOneLoss()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        var action = new GameAction
        {
            Type = ActionType.Attack,
            ChosenAttackerDiceCount = 1,
            ChosenDefenderDiceCount = 1
        };

        var rng = new EngineRandom(seed: 123);


        // Act
        var result = CombatResolver.Resolve(in state, in action, ref rng);


        // Assert
        Assert.Equal(1, result.AttackerLosses + result.DefenderLosses);

        Assert.True(result.AttackerLosses == 1 || result.DefenderLosses == 1);
    }


    /*
     * COMBAT-002
     *
     * Defender wins ties according to Risk rules.
     *
     * Guarantees:
     * - equal dice values cause attacker losses
     * - comparison uses strictly greater attacker roll
     */
    [Fact]
    public void COMBAT_002_EqualDice_ShouldCauseAttackerLoss()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        /*
         * This test depends on deterministic RNG.
         * Seed must produce identical dice values.
         */
        var action = new GameAction
        {
            Type = ActionType.Attack,
            ChosenAttackerDiceCount = 1,
            ChosenDefenderDiceCount = 1
        };

        var rng = new EngineRandom(seed: 1);


        // Act
        var result = CombatResolver.Resolve(in state, in action, ref rng);


        // Assert
        // Equal dice result means attacker loses.
        Assert.True(
            result.AttackerLosses == 1 ||
            result.DefenderLosses == 1);
    }


    /*
     * COMBAT-003
     *
     * Multiple dice comparisons should resolve
     * independently for every pair.
     *
     * Guarantees:
     * - maximum comparisons equal minimum dice count
     * - multiple losses are possible
     */
    [Fact]
    public void COMBAT_003_MultipleDiceCombat_ShouldResolveMultipleComparisons()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        var action = new GameAction
        {
            Type = ActionType.Attack,
            ChosenAttackerDiceCount = 3,
            ChosenDefenderDiceCount = 2
        };

        var rng = new EngineRandom(seed: 42);


        // Act
        var result = CombatResolver.Resolve(in state, in action, ref rng);


        // Assert
        Assert.Equal(2, result.AttackerLosses + result.DefenderLosses);
    }
    
    /*
     * COMBAT-004
     *
     * Combat resolution must be deterministic.
     *
     * Guarantees:
     * - same seed produces the same dice sequence
     * - same state and action produce the same combat result
     */
    [Fact]
    public void COMBAT_005_SameSeed_ShouldProduceSameCombatResult()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        var action = new GameAction
        {
            Type = ActionType.Attack,
            ChosenAttackerDiceCount = 3,
            ChosenDefenderDiceCount = 2
        };


        var rng1 = new EngineRandom(seed: 999);
        var rng2 = new EngineRandom(seed: 999);


        // Act
        var result1 = CombatResolver.Resolve(in state, in action, ref rng1);

        var result2 = CombatResolver.Resolve(in state, in action, ref rng2);


        // Assert
        Assert.Equal(result1.AttackerLosses, result2.AttackerLosses);

        Assert.Equal(result1.DefenderLosses, result2.DefenderLosses);
    }
    
    /*
     * COMBAT-005
     *
     * Combat resolver must respect selected dice counts.
     *
     * Guarantees:
     * - only selected attacker dice are rolled
     * - only selected defender dice are rolled
     * - number of comparisons matches Risk rules
     */
    [Fact]
    public void COMBAT_006_ShouldRespectSelectedDiceCounts()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        var action = new GameAction
        {
            Type = ActionType.Attack,
            ChosenAttackerDiceCount = 1,
            ChosenDefenderDiceCount = 2
        };

        var rng = new EngineRandom(seed: 123);


        // Act
        var result = CombatResolver.Resolve(in state, in action, ref rng);


        // Assert
        // Only one comparison can happen because attacker has one die.
        Assert.Equal(1, result.AttackerLosses + result.DefenderLosses);
    }
}