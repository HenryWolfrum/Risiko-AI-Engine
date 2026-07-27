namespace RiskEngine.Tests.Mutation;

using RiskEngine;
using RiskEngine.Mutation;
using Xunit;

public class ReinforceMutatorTests
{
    /*
     * MUTATE-REINFORCE-001
     *
     * Reinforcement should add troops
     * to the selected territory.
     *
     * Guarantees:
     * - target territory troop count increases
     */
    [Fact]
    public void MUTATE_REINFORCE_001_ShouldAddTroopsToTerritory()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);


        var action = new GameAction
        {
            TargetTerritory = 0,
            TroopCount = 3
        };


        // Act
        ReinforceMutator.Apply(ref state, in action);


        // Assert
        Assert.Equal(
            8,
            GameStateHelper.GetTerritoryTroops(in state, 0));
    }


    /*
     * MUTATE-REINFORCE-002
     *
     * Reinforcement should consume
     * available player troops.
     *
     * Guarantees:
     * - player reinforcement pool decreases
     */
    [Fact]
    public void MUTATE_REINFORCE_002_ShouldReducePlayerTroopPool()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetPlayerTroopsToPlace(ref state, 0, 10);


        var action = new GameAction
        {
            TargetTerritory = 0,
            TroopCount = 4
        };


        // Act
        ReinforceMutator.Apply(ref state, in action);


        // Assert
        Assert.Equal(
            6,
            GameStateHelper.GetPlayerTroopsToPlace(in state, 0));
    }


    /*
     * MUTATE-REINFORCE-003
     *
     * Reinforcement should only modify
     * the selected territory.
     *
     * Guarantees:
     * - unrelated territories remain unchanged
     */
    [Fact]
    public void MUTATE_REINFORCE_003_ShouldNotModifyOtherTerritories()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 7);


        var action = new GameAction
        {
            TargetTerritory = 0,
            TroopCount = 3
        };


        // Act
        ReinforceMutator.Apply(ref state, in action);


        // Assert
        Assert.Equal(
            8,
            GameStateHelper.GetTerritoryTroops(in state, 0));

        Assert.Equal(
            7,
            GameStateHelper.GetTerritoryTroops(in state, 1));
    }


    /*
     * MUTATE-REINFORCE-004
     *
     * Multiple reinforcement actions should
     * result in the accumulated final state.
     *
     * Guarantees:
     * - state transitions are consistent
     * - repeated mutations work correctly
     */
    [Fact]
    public void MUTATE_REINFORCE_004_MultipleReinforcementsShouldAccumulate()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 2);


        var firstAction = new GameAction
        {
            TargetTerritory = 0,
            TroopCount = 3
        };

        var secondAction = new GameAction
        {
            TargetTerritory = 0,
            TroopCount = 4
        };


        // Act
        ReinforceMutator.Apply(ref state, in firstAction);
        ReinforceMutator.Apply(ref state, in secondAction);


        // Assert
        Assert.Equal(9, GameStateHelper.GetTerritoryTroops(in state, 0));
    }
}