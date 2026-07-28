using RiskEngine.Tests.Helpers;

namespace RiskEngine.Tests.Validation;

public class FortifyRulesTests
{
    /*
     * FORTIFY-001
     *
     * A valid fortification between two connected
     * owned territories should be accepted.
     *
     * Guarantees:
     * - source and target are owned
     * - a valid path exists
     * - troop count is legal
     */
    [Fact]
    public void FORTIFY_001_ValidFortification_ShouldBeAccepted()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 0);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 2);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 3
        };

        // Act
        var result = FortifyRules.Validate(in state, in action, layout.Map);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(EngineError.None, result.Error);
    }
    
    /*
     * FORTIFY-002
     *
     * Source territory must exist on the map.
     *
     * Guarantees:
     * - invalid source territory ids are rejected
     */
    [Fact]
    public void FORTIFY_002_InvalidSourceTerritory_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        var action = new GameAction
        {
            SourceTerritory = layout.Map.TerritoryCount,
            TargetTerritory = 0,
            TroopCount = 1
        };

        // Act
        var result = FortifyRules.Validate(
            in state,
            in action,
            layout.Map);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.InvalidTerritory, result.Error);
    }
    
    /*
     * FORTIFY-003
     *
     * Target territory must exist on the map.
     *
     * Guarantees:
     * - invalid target territory ids are rejected
     */
    [Fact]
    public void FORTIFY_003_InvalidTargetTerritory_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = layout.Map.TerritoryCount,
            TroopCount = 1
        };

        // Act
        var result = FortifyRules.Validate(
            in state,
            in action,
            layout.Map);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.InvalidTerritory, result.Error);
    }
    
    /*
     * FORTIFY-004
     *
     * Source and target territory
     * must not be identical.
     *
     * Guarantees:
     * - self fortification is rejected
     */
    [Fact]
    public void FORTIFY_004_SameSourceAndTarget_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 0,
            TroopCount = 1
        };

        // Act
        var result = FortifyRules.Validate(in state, in action, layout.Map);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.InvalidTarget, result.Error);
    }
    
    
    /*
     * FORTIFY-006
     *
     * Fortification requires a connected path
     * through territories owned by the active player.
     *
     * Guarantees:
     * - disconnected territories are rejected
     * - path validation is enforced
     */
    [Fact]
    public void FORTIFY_006_DisconnectedTerritories_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);
        GameStateHelper.SetTerritoryOwner(ref state, 3, 0);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);
        GameStateHelper.SetTerritoryTroops(ref state, 3, 3);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 3,
            TroopCount = 2
        };

        // Act
        var result = FortifyRules.Validate(in state, in action, layout.Map);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.NoPathFound, result.Error);
    }
    
    /*
     * FORTIFY-007
     *
     * A player must leave at least one troop
     * behind when fortifying.
     *
     * Guarantees:
     * - moving all troops is rejected
     * - source territory always retains one troop
     */
    [Fact]
    public void FORTIFY_007_MovingAllTroops_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 0);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 2);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 5
        };

        // Act
        var result = FortifyRules.Validate(in state, in action, layout.Map);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.NotEnoughTroops, result.Error);
    }
    
    /*
     * FORTIFY-008
     *
     * Source territory must belong
     * to the active player.
     *
     * Guarantees:
     * - enemy source territories are rejected
     */
    [Fact]
    public void FORTIFY_008_SourceNotOwned_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 1);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 0);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 1
        };

        // Act
        var result = FortifyRules.Validate(
            in state,
            in action,
            layout.Map);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.TerritoryNotOwned, result.Error);
    }
    
    
    /*
     * FORTIFY-009
     *
     * Target territory must belong
     * to the active player.
     *
     * Guarantees:
     * - enemy target territories are rejected
     */
    [Fact]
    public void FORTIFY_009_TargetNotOwned_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 1
        };

        // Act
        var result = FortifyRules.Validate(
            in state,
            in action,
            layout.Map);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.TerritoryNotOwned, result.Error);
    }
}