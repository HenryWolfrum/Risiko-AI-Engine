using RiskEngine.State.Mutation;

namespace RiskEngine.State.Tests.Mutation;

public class FortifyMutatorTests
{
    /*
     * MUTATE-FORTIFY-001
     *
     * Fortification should remove troops
     * from the source territory.
     *
     * Guarantees:
     * - source troop count decreases
     */
    [Fact]
    public void MUTATE_FORTIFY_001_ShouldRemoveTroopsFromSourceTerritory()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 10);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 3
        };

        // Act
        FortifyMutator.Apply(ref state, in action);

        // Assert
        Assert.Equal(7, GameStateHelper.GetTerritoryTroops(in state, 0));
    }
    
    /*
     * MUTATE-FORTIFY-002
     *
     * Fortification should add troops
     * to the target territory.
     *
     * Guarantees:
     * - target troop count increases
     */
    [Fact]
    public void MUTATE_FORTIFY_002_ShouldAddTroopsToTargetTerritory()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 1, 5);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 3
        };

        // Act
        FortifyMutator.Apply(ref state, in action);

        // Assert
        Assert.Equal(
            8,
            GameStateHelper.GetTerritoryTroops(in state, 1));
    }
    
    
    /*
     * MUTATE-FORTIFY-003
     *
     * Fortification should move troops
     * between both territories.
     *
     * Guarantees:
     * - source troop count decreases
     * - target troop count increases
     */
    [Fact]
    public void MUTATE_FORTIFY_003_ShouldMoveTroopsBetweenTerritories()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 10);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 2);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 4
        };

        // Act
        FortifyMutator.Apply(ref state, in action);

        // Assert
        Assert.Equal(
            6,
            GameStateHelper.GetTerritoryTroops(in state, 0));

        Assert.Equal(
            6,
            GameStateHelper.GetTerritoryTroops(in state, 1));
    }
    
    
    /*
     * MUTATE-FORTIFY-004
     *
     * Fortification should preserve
     * the total troop count.
     *
     * Guarantees:
     * - total troop count remains constant
     */
    [Fact]
    public void MUTATE_FORTIFY_004_ShouldPreserveTotalTroopCount()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 9);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 4);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 5
        };

        var totalBefore =
            GameStateHelper.GetTerritoryTroops(in state, 0) +
            GameStateHelper.GetTerritoryTroops(in state, 1);

        // Act
        FortifyMutator.Apply(ref state, in action);

        var totalAfter =
            GameStateHelper.GetTerritoryTroops(in state, 0) +
            GameStateHelper.GetTerritoryTroops(in state, 1);

        // Assert
        Assert.Equal(totalBefore, totalAfter);
    }
    
    /*
     * MUTATE-FORTIFY-005
     *
     * Fortification should only modify
     * the participating territories.
     *
     * Guarantees:
     * - unrelated territories remain unchanged
     */
    [Fact]
    public void MUTATE_FORTIFY_005_ShouldNotModifyUnrelatedTerritories()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(3);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 8);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 4);
        GameStateHelper.SetTerritoryTroops(ref state, 2, 7);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 2
        };

        // Act
        FortifyMutator.Apply(ref state, in action);

        // Assert
        Assert.Equal(7, GameStateHelper.GetTerritoryTroops(in state, 2));
    }
    
    
    /*
     * MUTATE-FORTIFY-006
     *
     * Fortification should support
     * moving a single troop.
     *
     * Guarantees:
     * - exactly one troop is moved
     */
    [Fact]
    public void MUTATE_FORTIFY_006_ShouldMoveSingleTroop()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 3);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 1
        };

        // Act
        FortifyMutator.Apply(ref state, in action);

        // Assert
        Assert.Equal(4, GameStateHelper.GetTerritoryTroops(in state, 0));

        Assert.Equal(4, GameStateHelper.GetTerritoryTroops(in state, 1));
    }
}