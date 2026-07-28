using RiskEngine.Mutation;

namespace RiskEngine.Tests.Mutation;

public class ConquerMutatorTests
{
    /*
     * MUTATE-CONQUER-001
     *
     * Conquering a territory should remove
     * the moved troops from the source territory.
     *
     * Guarantees:
     * - source troop count decreases
     */
    [Fact]
    public void MUTATE_CONQUER_001_ShouldRemoveTroopsFromSourceTerritory()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryTroops(ref state, 0, 8);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            ConquerTroopCount = 3
        };

        // Act
        ConquerMutator.Apply(ref state, in action);

        // Assert
        Assert.Equal(5, GameStateHelper.GetTerritoryTroops(in state, 0));
    }
    
    
    /*
     * MUTATE-CONQUER-002
     *
     * Conquering a territory should transfer
     * ownership to the active player.
     *
     * Guarantees:
     * - target territory owner changes
     */
    [Fact]
    public void MUTATE_CONQUER_002_ShouldTransferTerritoryOwnership()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            ConquerTroopCount = 3
        };

        // Act
        ConquerMutator.Apply(ref state, in action);

        // Assert
        Assert.Equal(0, GameStateHelper.GetTerritoryOwner(in state, 1));
    }
    
    
    /*
     * MUTATE-CONQUER-003
     *
     * Conquering a territory should place
     * moved troops onto the target territory.
     *
     * Guarantees:
     * - target troop count equals conquer troop count
     */
    [Fact]
    public void MUTATE_CONQUER_003_ShouldPlaceTroopsOnTargetTerritory()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            ConquerTroopCount = 4
        };

        // Act
        ConquerMutator.Apply(ref state, in action);

        // Assert
        Assert.Equal(4, GameStateHelper.GetTerritoryTroops(in state, 1));
    }
    
    /*
     * MUTATE-CONQUER-004
     *
     * Conquering a territory should update
     * both territories in a single mutation.
     *
     * Guarantees:
     * - source troop count decreases
     * - target troop count is updated
     */
    [Fact]
    public void MUTATE_CONQUER_004_ShouldUpdateBothTerritories()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryTroops(ref state, 0, 9);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            ConquerTroopCount = 4
        };

        // Act
        ConquerMutator.Apply(ref state, in action);

        // Assert
        Assert.Equal(5, GameStateHelper.GetTerritoryTroops(in state, 0));

        Assert.Equal(4, GameStateHelper.GetTerritoryTroops(in state, 1));
    }
    
    /*
     * MUTATE-CONQUER-005
     *
     * Conquering a territory should only
     * modify the participating territories.
     *
     * Guarantees:
     * - unrelated territories remain unchanged
     */
    [Fact]
    public void MUTATE_CONQUER_005_ShouldNotModifyUnrelatedTerritories()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(3);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryTroops(ref state, 0, 8);
        GameStateHelper.SetTerritoryTroops(ref state, 2, 7);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            ConquerTroopCount = 3
        };

        // Act
        ConquerMutator.Apply(ref state, in action);

        // Assert
        Assert.Equal(7, GameStateHelper.GetTerritoryTroops(in state, 2));
    }
}