using RiskEngine.Actions;
using RiskEngine.Combat;
using RiskEngine.Constants;
using RiskEngine.Random;
using RiskEngine.State;

namespace RiskEngine.Tests.Combat;

public class CombatResolverTests
{
    /*
     * COMBAT-001
     *
     * Dice are compared according to the combat rules.
     *
     * Guarantees:
     * - a higher attacker roll causes a defender loss
     * - a tie causes an attacker loss
     * - a lower attacker roll causes an attacker loss
     */

    [Theory]
    [InlineData(6, 5, 0, 1)]
    [InlineData(4, 4, 1, 0)]
    [InlineData(2, 5, 1, 0)]
    public void COMBAT_001_DiceComparison_ShouldApplyCombatRules(byte attackerRoll, byte defenderRoll, int expectedAttackerLosses, int expectedDefenderLosses)
    {
        // Arrange
        byte[] attackerDice = [attackerRoll];
        byte[] defenderDice = [defenderRoll];

        // Act
        var result = CombatResolver.ResolveRolls(attackerDice, defenderDice);

        // Assert
        Assert.Equal(expectedAttackerLosses,result.AttackerLosses);
        Assert.Equal(expectedDefenderLosses,result.DefenderLosses);
    }

    /*
     * COMBAT-002
     *
     * The number of dice comparisons should equal
     * the smaller dice count.
     *
     * Guarantees:
     * - comparisons equal the minimum of both dice counts
     * - each comparison results in exactly one troop loss
     */
    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 1)]
    [InlineData(3, 2)]
    public void COMBAT_002_ComparisonCount_ShouldEqualSmallerDiceCount(int attackerCount, int defenderCount)
    {
        // Arrange
        byte[] attackerDice = Enumerable.Repeat((byte)4, attackerCount).ToArray();
        byte[] defenderDice = Enumerable.Repeat((byte)3, defenderCount).ToArray();

        var expectedComparisons = Math.Min(attackerCount, defenderCount);

        // Act
        var result = CombatResolver.ResolveRolls(attackerDice, defenderDice);

        // Assert
        Assert.Equal(expectedComparisons,result.AttackerLosses + result.DefenderLosses);
    }
    
    /*
     * COMBAT-003
     *
     * Dice should be sorted in descending order
     * before they are compared.
     *
     * Guarantees:
     * - one die remains unchanged
     * - two dice are sorted in descending order
     * - three dice are sorted in descending order
     */
    [Theory]
    [InlineData(new byte[] { 4 }, new byte[] { 2 }, new byte[] { 4 }, new byte[] { 2 })]
    [InlineData(new byte[] { 2, 6 }, new byte[] { 1, 5 }, new byte[] { 6, 2 },new byte[] { 5, 1 })]
    [InlineData(new byte[] { 2, 6, 3 }, new byte[] { 4, 1, 5 }, new byte[] { 6, 3, 2 }, new byte[] { 5, 4, 1 })]
    public void COMBAT_003_DiceSorting_ShouldSortDescending(byte[] attackerDice, byte[] defenderDice, byte[] expectedAttackerDice, byte[] expectedDefenderDice)
    {
        // Act
        CombatResolver.ResolveRolls(attackerDice, defenderDice);

        // Assert
        Assert.Equal(expectedAttackerDice, attackerDice);
        Assert.Equal(expectedDefenderDice, defenderDice);
    }
    
    /*
     * COMBAT-004
     *
     * Combat resolution should handle the maximum
     * valid attacker and defender dice counts.
     *
     * Guarantees:
     * - maximum valid dice counts are handled successfully
     * - the total number of losses equals the number of comparisons
     */
    [Fact]
    public void COMBAT_004_MaximumDiceCounts_ShouldResolveCorrectly()
    {
        // Arrange
        var action = new GameAction
        {
            Type = ActionType.Attack,
            ChosenAttackerDiceCount = EngineConstants.MAX_ATTACKER_DICE,
            ChosenDefenderDiceCount = EngineConstants.MAX_DEFENDER_DICE
        };

        var rng = new EngineRandom(seed: 123);

        var expectedComparisons = Math.Min(EngineConstants.MAX_ATTACKER_DICE, EngineConstants.MAX_DEFENDER_DICE);

        // Act
        var result = CombatResolver.Resolve(in action, ref rng);

        // Assert
        Assert.Equal(expectedComparisons,result.AttackerLosses + result.DefenderLosses);
    }

    /*
     * COMBAT-005
     *
     * Identical actions and initial RNG states should
     * produce identical combat results.
     *
     * Guarantees:
     * - identical seeds produce identical dice sequences
     * - identical inputs produce identical loss results
     */
    [Fact]
    public void COMBAT_005_SameSeed_ShouldProduceSameResult()
    {
        // Arrange
        var action = new GameAction
        {
            Type = ActionType.Attack,
            ChosenAttackerDiceCount = 3,
            ChosenDefenderDiceCount = 2
        };

        var rng1 = new EngineRandom(seed: 999);
        var rng2 = new EngineRandom(seed: 999);

        // Act
        var result1 = CombatResolver.Resolve(in action, ref rng1);
        var result2 = CombatResolver.Resolve(in action, ref rng2);

        // Assert
        Assert.Equal(result1.AttackerLosses, result2.AttackerLosses);
        Assert.Equal(result1.DefenderLosses, result2.DefenderLosses);
    }
    
    /*
     * COMBAT-006
     *
     * Losses should remain within valid bounds,
     * and their sum should equal the number of comparisons.
     *
     * Guarantees:
     * - attacker losses are within valid bounds
     * - defender losses are within valid bounds
     * - total losses equal the number of comparisons
     */
    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 1)]
    [InlineData(3, 2)]
    public void COMBAT_006_Losses_ShouldRemainWithinBounds(int attackerCount, int defenderCount)
    {
        // Arrange
        byte[] attackerDice = Enumerable.Repeat((byte)4, attackerCount).ToArray();
        byte[] defenderDice = Enumerable.Repeat((byte)3, defenderCount).ToArray();

        var comparisons = Math.Min(attackerCount, defenderCount);

        // Act
        var result = CombatResolver.ResolveRolls(attackerDice, defenderDice);

        var attackerLosses = (int)result.AttackerLosses;
        var defenderLosses = (int)result.DefenderLosses;

        // Assert
        Assert.InRange(attackerLosses, 0, comparisons);
        Assert.InRange(defenderLosses, 0, comparisons);

        Assert.Equal(comparisons, attackerLosses + defenderLosses);
    }
}