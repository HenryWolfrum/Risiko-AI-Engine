using RiskEngine.Validation;

namespace RiskEngine.Tests.Validation;

using RiskEngine;
using RiskEngine.Tests.Helpers;
using Xunit;

public class ReinforceRulesTests
{
    /*
     * REINFORCE-001
     *
     * A player should be able to place troops
     * on a territory they own.
     *
     * Guarantees:
     * - owned territory is accepted
     * - available troops are respected
     */
    [Fact]
    public void REINFORCE_001_PlayerShouldPlaceTroopsOnOwnedTerritory()
    {
        // Arrange
        var layout = TestLayoutBuilder.CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(layout.Config.PlayerCount);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetPlayerTroopsToPlace(ref state, 0, 5);


        var action = new GameAction
        {
            SourceTerritory = 0,
            TroopCount = 3
        };


        // Act
        var result = ReinforceRules.Validate(in state, in action, layout.Map);


        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(GameError.None, result.Error);
    }


    /*
     * REINFORCE-002
     *
     * A player cannot place troops
     * on an enemy territory.
     *
     * Guarantees:
     * - ownership validation works
     */
    [Fact]
    public void REINFORCE_002_PlayerCannotPlaceTroopsOnEnemyTerritory()
    {
        // Arrange
        var layout = TestLayoutBuilder.CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(layout.Config.PlayerCount);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 1);
        GameStateHelper.SetPlayerTroopsToPlace(ref state, 0, 5);


        var action = new GameAction
        {
            SourceTerritory = 0,
            TroopCount = 2
        };


        // Act
        var result = ReinforceRules.Validate(in state, in action, layout.Map);


        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(GameError.TerritoryNotOwned, result.Error);
    }


    /*
     * REINFORCE-003
     *
     * Placing zero troops should be rejected.
     *
     * Guarantees:
     * - empty reinforcement actions are invalid
     */
    [Fact]
    public void REINFORCE_003_ZeroTroopsShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder.CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(layout.Config.PlayerCount);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetPlayerTroopsToPlace(ref state, 0, 5);


        var action = new GameAction
        {
            SourceTerritory = 0,
            TroopCount = 0
        };


        // Act
        var result = ReinforceRules.Validate(in state, in action, layout.Map);


        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(GameError.InvalidTroopCount, result.Error);
    }


    /*
     * REINFORCE-004
     *
     * A player cannot place more troops
     * than currently available.
     *
     * Guarantees:
     * - reinforcement pool is respected
     */
    [Fact]
    public void REINFORCE_004_CannotPlaceMoreTroopsThanAvailable()
    {
        // Arrange
        var layout = TestLayoutBuilder.CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(layout.Config.PlayerCount);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetPlayerTroopsToPlace(ref state, 0, 2);


        var action = new GameAction
        {
            SourceTerritory = 0,
            TroopCount = 3
        };


        // Act
        var result = ReinforceRules.Validate(in state, in action, layout.Map);


        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(GameError.NotEnoughTroops, result.Error);
    }


    /*
     * REINFORCE-005
     *
     * Invalid territory ids should be rejected.
     *
     * Guarantees:
     * - validation uses actual map size
     */
    [Fact]
    public void REINFORCE_005_InvalidTerritoryShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder.CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(layout.Config.PlayerCount);

        var action = new GameAction
        {
            SourceTerritory = layout.Map.TerritoryCount,
            TroopCount = 1
        };


        // Act
        var result = ReinforceRules.Validate(in state, in action, layout.Map);


        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(GameError.InvalidTerritory, result.Error);
    }
}